using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        //to access all of the helper functions that we have with .net identity
        //we have to inject something with DI
        //now 1st when we register or login the user we need to update our DB so we need AppDbContext
        private readonly AppDbContext _db;

        //then inject helper methods
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, IJwtTokenGenerator jwtTokenGenerator, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                //before we assign any particular role to user we need to make sure that
                //that particular role is there in database
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())  //used getawaiter and getresult because it's async method
                {
                    //create role if does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                //now assign a user a particular role
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            //check password
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password); 

            if(user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            //if user not found, we need to generate the jwt token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user,roles);

            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                //this will create the user and add that into the db 
                var result = await _userManager.CreateAsync(user,registrationRequestDto.Password);

                if(result.Succeeded)
                {
                    //if registration successful that means we can access AppDbContext and we can retriev
                    //the user based on the email
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    //now based on that user we can create or populate that user

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return ""; //if success then return empty string
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }

                //and all other things like password hashing is done in the .net identity itself


            }catch (Exception ex)
            {

            }
            //if there is any exception return the new Userdto
            return "error encountered";
        }
    }
}
