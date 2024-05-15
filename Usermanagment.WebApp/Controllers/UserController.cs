using Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Usermanagment.Entities;
using Usermanagment.WebApp.AppCode;

namespace Usermanagment.WebApp.Controllers
{
    [Authorize(AuthenticationSchemes = SystemKeys.AuthenticationScheme)]
    public class UserController : Controller
    {
        public async Task<IActionResult> Profile()
        {
            UserColumn user = new();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://localhost:5109");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {User.GetLoggedInUserToken()}");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    HttpResponseMessage response = await client.PostAsync($"/api/User/GetUserByName", null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        user = JsonConvert.DeserializeObject<UserColumn>(responseData);
                       
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            return View(user);
        }
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
