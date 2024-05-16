using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.Domain.Base;
using UserManagement.Domain.Interfaces;
using UserManagement.Entities;
using UserManagement.Infrastructure.Services;
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
        private readonly IGoogleAuthenticatorManager _gAuthManager;
        private readonly IAuditTrail<IRow> _auditTrail;
        public AccountController(ISignInManager signInManager, IUserService userService, IPasswordManager PasswordManager, ITokenManager tokenManager, IGoogleAuthenticatorManager gAuthManager, IAuditTrail<IRow> auditTrail)
        {
            _signInManager = signInManager;
            _userService = userService;
            _PasswordManager = PasswordManager;
            _tokenManager = tokenManager;
            _gAuthManager = gAuthManager;
            _auditTrail = auditTrail;
        }

        [HttpPost("[action]")]
        public async Task<IResponse<AuthenticateResponse>> SignIn(string userName, string password, string authCode="")
        {
            IResponse<AuthenticateResponse> res = new Response<AuthenticateResponse>
            {
                ResponseText = "Invalid Credentials"
            };
            var user = await _userService.GetByUserName(userName);
            if (user == null)
            {
                return res;
            }
            var signinRes = await _signInManager.SignInAsync(user.ToApplicationUser(), password);
            /* Genrate JWT Tokan Here */
            if (signinRes.StatusCode == ResponseStatus.Success && user.Id > 0)
            {
                if (user.GAuthRequired)
                {
                    if (string.IsNullOrEmpty(authCode))
                    {
                        res.StatusCode = ResponseStatus.GAuthRequired;
                        res.ResponseText = "2 FA Required.Please authenticate with google authenticator";
                        return res;
                    }
                    if (!_gAuthManager.Verify(authCode, user.GAuthAccountKey))
                    {
                        res.ResponseText = "Invalid Google Auth Code";
                        return res;
                    }
                }
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

        /// <summary>
        /// Test AuditTrail For Delete 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("[action]/{userName}")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            var user = await _userService.GetByUserName(userName);
            if (user != null)
            {
                _ = _auditTrail.DeleteAsync(user);
            }
            return Ok("Delete Successfully!!!");
        }
    }
}