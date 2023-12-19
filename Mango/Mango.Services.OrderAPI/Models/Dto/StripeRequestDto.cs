namespace Mango.Services.OrderAPI.Models.Dto
{
    public class StripeRequestDto
    {
        public string? StripeSessionId { get; set; }
        public string? StripeSessionUrl { get; set; }
        public string ApprovedUrl { get; set; }  //redirects once the payment is successful
        public string CancelUrl { get; set; }   //if user clicks back button

        public OrderHeaderDto OrderHeader { get; set; }  //this will have all the details for the order which will be displayed on stripe page
        
    }
}
