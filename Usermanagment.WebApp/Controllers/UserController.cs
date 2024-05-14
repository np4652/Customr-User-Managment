using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Usermanagment.Entities;
using Usermanagment.WebApp.AppCode;

namespace Usermanagment.WebApp.Controllers
{
    [Authorize(AuthenticationSchemes = SystemKeys.AuthenticationScheme)]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Grid()
        {
            return PartialView(new List<UserColumn>());
        }
    }
}
