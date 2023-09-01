using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        
        //return type should be async as in the coupon service
        public async Task<IActionResult> CouponIndex()
        {
            //now we want to display all the coupons
            List<CouponDto>? list = new();

            //this should get all the coupons
            ResponseDto? response = await _couponService.GetAllCouponsAsync();

            if (response != null && response.IsSuccess) 
            {
                //we have result is in object so we have to deserialize it
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                List<CouponDto>? list = new();

                //this should get all the coupons
                ResponseDto? response = await _couponService.CreateCouponsAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon Created Successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDto? response = await _couponService.GetAllCouponsByIdAsync(couponId);

            if (response != null && response.IsSuccess)
            {
                //we have result is in object so we have to deserialize it
                CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto couponDto)
        {
            ResponseDto? response = await _couponService.DeleteCouponsAsync(couponDto.CouponId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon Deleted Successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(couponDto);
        }
    }
}
