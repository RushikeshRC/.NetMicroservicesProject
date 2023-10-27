using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        //This is responsible to load the product from the product API
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
