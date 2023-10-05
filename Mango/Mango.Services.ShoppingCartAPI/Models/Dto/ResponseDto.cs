namespace Mango.Services.ShoppingCartAPI.Models.Dto
{
    public class ResponseDto  //we need common response for every API for that purpose
    {
        public object? Result { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string Message { get; set; } = " ";
    }
}
