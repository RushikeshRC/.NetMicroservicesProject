using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Utility;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;

        public OrderAPIController(AppDbContext db, IProductService productService, IMapper mapper)
        {
            _db = db;
            _productService = productService;
            _mapper = mapper;
            this._response = new ResponseDto();
        }

        //when we make the post req on createorder we will pass that complete cartdto
        //and from that cartdto we will be using Automapper to convert that to orderheader
        //populate some additional details then create the order header and orderdetail
        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)  //because from the page we're on shopping cart we have cartdto
        {
            try
            {
                //from cartdto we can extract orderheaderdto and get using automapper
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);     //this will do conversion based on what we defined in mapperconfig
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = StaticDetails.Status_Pending;  //initial status will be pending
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                //now create the order
                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                //return
                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDto;

            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    //line items will be the items that we will display on checkout page
                    LineItems = new List<SessionLineItemOptions>(),                       
                    Mode = "payment",
                };

                //since it can be multiple items so
                foreach(var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),  //$20.99 -> 2099 in stripe
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        //once we defined the qty here, it will automatically display the pricedata along with the grandtotal
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);  //this is the stripe session and it requires stripesessionid and url
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _db.SaveChanges();
                _response.Result = stripeRequestDto;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
