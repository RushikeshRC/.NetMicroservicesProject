namespace Mango.Services.AuthAPI.Models.Dto
{
    public class ResponseDto  //we need common response for API for that purpose
    {
        public object? Result { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string Message { get; set; } = " ";
    }
}
