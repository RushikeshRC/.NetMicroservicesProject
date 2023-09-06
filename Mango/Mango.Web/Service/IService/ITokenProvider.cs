namespace Mango.Web.Service.IService
{
    public interface ITokenProvider
    {
        //this is to stroe the token in cookies 
        void setToken(string token);  //sets the token
        string? getToken(); //gets the token
        void clearToken(); //clears the token
    }
}
