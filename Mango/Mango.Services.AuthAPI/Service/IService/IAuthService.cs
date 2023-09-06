using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        //when user registering return type will be UserDTO
        //and we receive the input parameter as RegistrationRequestDto
        Task<string> Register(RegistrationRequestDto registrationRequestDto);

        //when user is logging in the return type will be LoginResponseDto
        //and the parameter we will receive be LoginRequestDto
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);

        //we will assign a role based on Email and Rolename
        Task<bool> AssignRole(string email, string roleName);
    }
}
