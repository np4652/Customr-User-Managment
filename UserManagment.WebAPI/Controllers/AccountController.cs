using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.Domain.Base;
using UserManagement.Domain.Interfaces;
using UserManagement.Entities;
using Usermanagment.Entities;
using Usermanagment.Entities.DTOs;
using UserManagment.WebAPI.Modals;

namespace UserManagment.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISignInManager _signInManager;
        private readonly IPasswordManager _PasswordManager;
        private readonly ITokenManager _tokenManager;
        public AccountController(ISignInManager signInManager, IUserService userService, IPasswordManager PasswordManager, ITokenManager tokenManager)
        {
            _signInManager = signInManager;
            _userService = userService;
            _PasswordManager = PasswordManager;
            _tokenManager = tokenManager;
        }

        [HttpPost("[action]")]
        public async Task<IResponse<AuthenticateResponse>> SignIn(string userName, string password)
        {
            IResponse<AuthenticateResponse> res = new Response<AuthenticateResponse>();
            var user = await _userService.GetByUserName(userName);
            if (user == null)
            {
                return res;
            }
            var signinRes = await _signInManager.SignInAsync(user.ToApplicationUser(), password);
            /* Genrate JWT Tokan Here */
            if (signinRes.StatusCode == ResponseStatus.Success && user.Id > 0)
            {
                var claims = new[] {
                    new Claim(ClaimTypesExtension.UserName, user.UserName),
                    new Claim(ClaimTypesExtension.Id, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };
                var token = _tokenManager.GenerateAccessToken(claims);
                var authResponse = new AuthenticateResponse(user, token);
                res = new Response<AuthenticateResponse>(authResponse);
            }
            /* End */
            return res;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SignUP(UserSignupRequest request)
        {
            request.Role = UserRole.User;
            return Ok(await _userService.Add(new ServiceRequest<UserRow> { param = request.ToUserRow(_PasswordManager) }));
        }
    }
}
