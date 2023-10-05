using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class CartHeader
    {
        //This contains info like what is userId, couponCode,
        //what is cart total and if any coupon code is applied then what is the discount

        [Key]
        public int CartHeaderId { get; set; }  //for one user there will always be one record inside the cartheaderId

        public string? UserId { get; set; }

        public string? CouponCode { get; set; }  //user might apply the coupon 

        //Now these 2 properties we do not want to store in database 
        //we only want them for display puropose 
        //means we will dynamically calculate them based on shopping cart and cart detail

        [NotMapped]
        public double Discount { get; set; } //if couponcode is applied we will also have some discount

        [NotMapped]
        public double CartTotal { get; set; } 

    }
}
