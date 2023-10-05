using Mango.Services.ShoppingCartAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class CartDetails
    {
        //It contains list of all the products that are added for that user in the
        //shopping cart and what is the count of that product 

        [Key]
        public int CartDetailsId { get; set; }

        public int CartHeaderId { get; set; }

        [ForeignKey("CartHeaderId")]
        public CartHeader CartHeader { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        public ProductDto Product { get; set; }

        public int Count { get; set; }
    }
}
