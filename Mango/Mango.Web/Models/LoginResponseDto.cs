namespace Mango.Web.Models
{ 
    public class LoginResponseDto
    {
        public UserDto User { get; set; }  //we get the loggedin user info here

        public string Token { get; set; } //and to check the correct user has logged in we have Jwttoken here
    }
}
