using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        //injecting token provider
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto responseDto = await _authService.LoginAsync(obj);

            if(responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                await SignInUser(loginResponseDto);

                //when we get loginResponsedto back we will set the token provider
                _tokenProvider.setToken(loginResponseDto.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto.Message;
                return View(obj);                
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=StaticDetails.RoleAdmin,Value=StaticDetails.RoleAdmin},
                new SelectListItem{Text=StaticDetails.RoleCustomer,Value=StaticDetails.RoleCustomer},
            };
            
            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            //we will get response back which will create or register user
            ResponseDto result = await _authService.RegisterAsync(obj);

            //once we register the user we need to assign the role
            ResponseDto assignRole;

            if(result!=null && result.IsSuccess)
            {
                //this means the registration was successfull
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = StaticDetails.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(obj);  //this will create the role if it doesn't exist
                                                                       //or if exist then it will assign that role to the user

                if(assignRole!=null && assignRole.IsSuccess) 
                {
                    TempData["success"] = "Registration Successful..";
                    //redirect to login
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = result.Message;
            }

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=StaticDetails.RoleAdmin,Value=StaticDetails.RoleAdmin},
                new SelectListItem{Text=StaticDetails.RoleCustomer,Value=StaticDetails.RoleCustomer},
            };

            ViewBag.RoleList = roleList;

            return View(obj);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.clearToken();
            return RedirectToAction("Index", "Home");
        }
            
        private async Task SignInUser(LoginResponseDto model)
        {
            //in the parameters of claimsPrincipal we can also pass Identity
            //so in that identity we have pass few claimtypes which are there in .net identity
            //then you have authentication checkedIn

            var handler = new JwtSecurityTokenHandler();

            //after this we need jwttoken and this will read the token
            var jwt = handler.ReadJwtToken(model.Token);

            //now from that token we want to extract all the claims that we added
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, 
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
