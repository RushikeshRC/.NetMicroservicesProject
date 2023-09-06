using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using static Mango.Web.Utility.StaticDetails;

namespace Mango.Web.Service
{
    //We have to add this base service in the programcs file
    //because only then DI will inject that base service 
    //when we needed it in any other class
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                //when we have to make an API call we need httpclient
                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                //when we add authentication we have to add token here

                if(withBearer)
                {
                    var token = _tokenProvider.getToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");                }


                message.RequestUri = new Uri(requestDto.Url); //if it is GET req then its ok
                                                              //but if its Post or put req we need to serialize that Data received in reqDto and 
                                                              //add that to message.content
                if (requestDto.Data != null)
                {
                    //serialize the data
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }
                HttpResponseMessage? apiresponse = null;

                switch (requestDto.ApiType) //based on the API type we are setting the message
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                //now get the response back
                apiresponse = await client.SendAsync(message);

                switch (apiresponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };

                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access denied" };

                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };

                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };

                    default:
                        //here we have to retrieve the content from apiresponse
                        var apicontent = await apiresponse.Content.ReadAsStringAsync();

                        //and then we deserialize that to responsedto
                        var apiresponsedto = JsonConvert.DeserializeObject<ResponseDto>(apicontent);
                        return apiresponsedto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}
