using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            //if you get the coupon you need to convert that to coupondto and vice vers
            //with automapper it is possible, as long as propery names are same in both it will do the automatic mapping

            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>(); //define the source and the destination here
                config.CreateMap<Coupon, CouponDto>();
            });
            return mappingConfig;

            //now register this in services i.e in program.cs
        }

    }
}
