using Helpers;
using Helpers.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Usermanagment.WebApp.Models;

namespace Usermanagment.WebApp.Views.User
{
    public class TwoFactorModel : PageModel
    {
        public TwoFactorSetupVM TwoFactorSetupVM { get; set; }
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
                        TwoFactorSetupVM = JsonConvert.DeserializeObject<TwoFactorSetupVM>(responseData);
                        TwoFactorSetupVM.AccountSecretKey = AppUtility.O.AddSpacesAfterEveryNCharacters(TwoFactorSetupVM.AccountSecretKey, 4);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}