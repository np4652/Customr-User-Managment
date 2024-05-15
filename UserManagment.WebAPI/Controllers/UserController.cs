using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Interfaces;
using Usermanagment.Entities;
using Helpers.Extensions;
using GoogleAuthenticator;
using UserManagement.Infrastructure.Services;

namespace UserManagment.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IGoogleAuthenticatorManager _gAuthManager;
        public UserController(IUserService userService, IGoogleAuthenticatorManager gAuthManager)
        {
            _userService = userService;
            _gAuthManager = gAuthManager;
        }

        [HttpPost("[action]")]
        public async Task<UserColumn> GetUserByName(string userName = "")
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = User.GetLoggedInUserName();
            }
            return await _userService.GetByUserName(userName);
        }

        [HttpPost("[action]")]
        public SetupCode SetupTwoFactor()
        {
            return _gAuthManager.Setup(User.GetLoggedInUserName());
        }
    }
}
