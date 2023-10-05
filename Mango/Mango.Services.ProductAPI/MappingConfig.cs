using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            //if you get the coupon you need to convert that to coupondto and vice vers
            //with automapper it is possible, as long as propery names are same in both it will do the automatic mapping

            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap() ; //define the source and the destination here
            });
            return mappingConfig;

            //now register this in services i.e in program.cs
        }

    }
}
