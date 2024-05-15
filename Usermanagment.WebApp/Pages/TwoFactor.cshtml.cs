using Entities.DTOs;
using Helpers;
using Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;
using UserManagement.Domain.Base;
using Usermanagment.WebApp.Models;

namespace Usermanagment.WebApp.Views.User
{
    public class TwoFactorModel : PageModel
    {
        public GAuthenticatorResponse TwoFactorSetupVM { get; set; }
        public async Task OnGetAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://localhost:5109");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {User.GetLoggedInUserToken()}");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    HttpResponseMessage response = await client.PostAsync($"/api/User/SetupTwoFactor", null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        TwoFactorSetupVM = JsonConvert.DeserializeObject<GAuthenticatorResponse>(responseData);
                        TwoFactorSetupVM.Configuration.AccountSecretKey = AppUtility.O.AddSpacesAfterEveryNCharacters(TwoFactorSetupVM.Configuration.AccountSecretKey, 4);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostConfigure(string accountSecretKey, string authCode)
        {
            IResponse res = new Response();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://localhost:5109");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {User.GetLoggedInUserToken()}");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    string jsonContent = JsonConvert.SerializeObject(new
                    {
                        AuthCode = authCode,
                        AccountSecretKey = accountSecretKey
                    });
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync($"/api/User/Configure2FactorWithApp", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<Response>(responseData);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return new JsonResult(res);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostSetGAuthRequired(bool enable)
        {
            IResponse res = new Response();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://localhost:5109");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {User.GetLoggedInUserToken()}");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    HttpResponseMessage response = await client.PostAsync($"/api/User/SetGAuthRequired?enable={enable}", null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<Response>(responseData);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return new JsonResult(res);
        }
    }
}