using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Mango.Services.ShoppingCartAPI.Service
{
    //for reference refer the product API about how you have implemented the base service there
    public class ProductService : IProductService
    {
        //now in order to make the http call we need to inject httpfactory here
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");   //it should be similar to the name in program.cs file

            var response = await client.GetAsync($"/api/product");
            //once we get the response we will retrieve apicontent from there
            var apiContent = await response.Content.ReadAsStringAsync();
            //finally get the response back by deserializing
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (resp.IsSuccess)
            {
                //we will again deserialize to IEnumerable of ProductDto
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }
            //if resp is not successful then return the new list of products
            return new List<ProductDto>();
        }
    }
}
