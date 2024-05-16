using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Interfaces;
using Usermanagment.Entities;
using Helpers.Extensions;
using UserManagement.Infrastructure.Services;
using UserManagment.WebAPI.Modals;
using UserManagement.Domain.Base;
using UserManagement.Entities;
using Entities.DTOs;

namespace UserManagment.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IGoogleAuthenticatorManager _gAuthManager;
        private readonly IAuditTrail<IRow> _auditTrail;
        public UserController(IUserService userService, IGoogleAuthenticatorManager gAuthManager, IAuditTrail<IRow> auditTrail)
        {
            _userService = userService;
            _gAuthManager = gAuthManager;
            _auditTrail = auditTrail;
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
        public async Task<GAuthenticatorResponse> SetupTwoFactor()
        {
            var user = await _userService.GetByUserName(User.GetLoggedInUserName());
            var res = _gAuthManager.Setup(user.UserName, user.GAuthAccountKey);
            res.Enabled = user.GAuthRequired;
            return res;
        }

        [HttpPost("[action]")]
        public async Task<IResponse> Configure2FactorWithApp(ConfigTwoFactorReq req)
        {
            IResponse res = new Response();
            if (_gAuthManager.Verify(req.AuthCode, req.AccountSecretKey))
            {
                res = await _userService.SetGAuthAccountKey(User.GetLoggedInUserName(), req.AccountSecretKey);
            }
            return res;
        }

        [HttpPost("[action]")]
        public async Task<IResponse> SetGAuthRequired(bool enable)
        {
            var user = await _userService.GetByUserName(User.GetLoggedInUserName());
            var res = await _userService.SetGAuthRequired(User.GetLoggedInUserName(), enable);
            if (res.StatusCode == ResponseStatus.Success)
            {
                _ = _auditTrail.UpdateAsync(user, new UserRow
                {
                    Id = user.Id,
                    GAuthRequired = enable
                });
            }
            return await _userService.SetGAuthRequired(User.GetLoggedInUserName(), enable);
        }
    }
}
