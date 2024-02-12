using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;   
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            //load the new copy with all the records that are populated 
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();

            //reload the cart and send that to our API
            //populate the i/p fields that user added
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            

            ////now call the orderservice
            //var response = await _orderService.CreateOrder(cart); //make sure to pass cart and not the cartdto of parameter
            //OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            
            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                //get stripe session and redirect to stripe to place the order

                //rather than hardcoding prop we can do
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDto stripeRequestDto = new()
                {
                    //properties
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,  //url for stripe
                    CancelUrl = domain + "cart/checkout",
                    OrderHeader = orderHeaderDto
                };

                ////call the API
                //var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);

                ////now the response that we will get back from here in OrderAPI is striperequestDto in _response.Result so we need to Deserialize that
                //StripeRequestDto stripeResponseResult  = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));

                //Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);
                //return new StatusCodeResult(303);   //redirect to another page

                var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);
                
                StripeRequestDto stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));
                
                Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);
                
                return new StatusCodeResult(303);
            }

            return View();
        }

        
        public async Task<IActionResult> Confirmation(int orderId)
        {
            return View(orderId);
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);

            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            //when we apply coupon we need userid but we already have it in the cartindex 
            //var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            //that way when form is posted userid is automatically posted
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);

            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);

            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            //here 1st we have to find the userid of the loggedin user
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);

            if(response != null & response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();
        }

    }
}
