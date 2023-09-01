using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        //here we need the base class because that is responsible 
        //for handling all httprequest as it is base implementation for all services
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateCouponsAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = couponDto,
                Url = StaticDetails.CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseDto?> DeleteCouponsAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = StaticDetails.CouponAPIBase + "/api/coupon/"+id
            });
        }

        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CouponAPIBase+"/api/coupon"
            });
        }

        public async Task<ResponseDto?> GetAllCouponsByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CouponAPIBase + "/api/coupon/" + id
            });
        }

        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CouponAPIBase + "/api/coupon/GetByCode/"+couponCode
            });
        }

        public async Task<ResponseDto?> UpdateCouponsAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = couponDto,
                Url = StaticDetails.CouponAPIBase + "/api/coupon"
            });
        }
    }
}
