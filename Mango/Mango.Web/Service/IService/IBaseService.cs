using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    //as we are in the web project and we need to call CouponAPI
    //and when we add more APIs into the project that particular service will be
    //responsible to call all the other APIs that's why we created Service here
    //to make it more dynamic
    public interface IBaseService
    {
        //when we making any API call, in parameter we are passing RequestDto that we created
        //and we will get back the response back in ResponseDto
        //As we want this method async so Task
        Task<ResponseDto?> SendAsync(RequestDto requestDto); 
    }
}
