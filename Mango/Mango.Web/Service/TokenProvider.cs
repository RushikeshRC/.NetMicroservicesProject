using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Mango.Web.Service
{

    //note - we will use this token provider in auth controller when we login

    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void clearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.TokenCookie);
        }

        public string? getToken()
        {
            //retrieve the token
            string ? token = null;

            //this time we are requesting the token
            bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetails.TokenCookie, out token);
            //so if it has value it will assign that to hasToken

            return hasToken is true ? token : null;
        }

        public void setToken(string token)
        {
            //here we need key value pair in Append so in static details create a constant with name TokenCookie 
            _contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.TokenCookie, token);
        }
    }
}
