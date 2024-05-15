using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using UserManagement.Domain.Base;
using Usermanagment.Entities.DTOs;
using Usermanagment.WebApp.AppCode;
using DTOs = UserManagment.Entities.DTOs;
namespace Usermanagment.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string user, string password, string ReturnUrl)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return NotFound();
            }
            // Validate login credentials here and get user details.
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://localhost:5109");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    HttpResponseMessage response = await client.PostAsync($"/api/Account/SignIn?userName={user}&password={password}", null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<Response<AuthenticateResponse>>(responseData);
                        if (res.StatusCode == UserManagement.Entities.ResponseStatus.Success && res.Result != null)
                        {
                            var claims = new List<Claim>{
                                new Claim(ClaimTypes.NameIdentifier, res.Result.UserName),
                                new Claim(ClaimTypes.Name, res.Result.UserName),
                                new Claim(ClaimTypes.Role, res.Result.Role),
                                new Claim("Token",  res.Result.Token),
                        };
                            var claimsIdentity = new ClaimsIdentity(claims, SystemKeys.AuthenticationScheme);
                            await _httpContextAccessor.HttpContext.SignInAsync(SystemKeys.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties());
                            if (string.IsNullOrEmpty(ReturnUrl))
                                return Ok("Login Successfully");
                            else
                                return LocalRedirect(ReturnUrl);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            ViewBag.Message = "Invalid Credentials";
            return View();
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(DTOs.UserSignupRequest request)
        {
            IResponse res = new Response();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://localhost:5109");
                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer YourAccessToken");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    string jsonContent = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/Account/signup", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<Response>(responseData);
                        ViewBag.Message = res.ResponseText;
                        ViewBag.StatusCode = res.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            return View(res);
        }

        public async Task<IActionResult> SignOut()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return NotFound();
            }
            await _httpContextAccessor.HttpContext.SignOutAsync(SystemKeys.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
