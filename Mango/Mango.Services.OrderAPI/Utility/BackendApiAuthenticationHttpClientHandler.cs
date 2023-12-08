using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Mango.Services.OrderAPI.Utility
{
    //to pass our bearer token to the other request, its baasically on client side
    public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
    {
        //to access the bearer token we need httpcontextaccessor here

        private readonly IHttpContextAccessor _contextAccessor;

        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        //to access the token and add that autorization header
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
        //register this in program.cs
    }
}
