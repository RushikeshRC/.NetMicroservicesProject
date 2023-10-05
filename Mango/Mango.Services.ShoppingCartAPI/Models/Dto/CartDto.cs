namespace Mango.Services.ShoppingCartAPI.Models.Dto
{
    public class CartDto
    {
        //when we want to retrieve the shopping cart for a user 
        //that will have a one cart header and can have multiple cart details

        public CartHeaderDto CartHeader { get; set; }

        public IEnumerable<CartDetailsDto> CartDetails { get; set; }
    }
}
