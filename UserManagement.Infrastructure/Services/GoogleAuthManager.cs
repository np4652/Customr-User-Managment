using Entities.DTOs;
using GoogleAuthenticator;
using System.Text;

namespace UserManagement.Infrastructure.Services
{
    public interface IGoogleAuthenticatorManager
    {
        GAuthenticatorResponse Setup(string userId, string authenticatorKey = "");
        bool Verify(string googlePin, string accountSecretKey);
    }
    public class GoogleAuthenticatorManager : IGoogleAuthenticatorManager
    {
        private readonly string ProjectName;
        public GoogleAuthenticatorManager()
        {
            ProjectName = GetType().Assembly.FullName.Split(',')[0];
        }

        public GAuthenticatorResponse Setup(string userId, string authenticatorKey = "")
        {
            authenticatorKey = string.IsNullOrEmpty(authenticatorKey) ? GenrateSecretKey() : authenticatorKey;
            TwoFactorAuthenticator Authenticator = new TwoFactorAuthenticator();
            var SetupResult = Authenticator.GenerateSetupCode(ProjectName, userId, authenticatorKey);
            return new GAuthenticatorResponse
            {
                Configuration = new GAuthenticatorResponse.SetupCode
                {
                    Account = SetupResult.Account,
                    AccountSecretKey = SetupResult.AccountSecretKey,
                    ManualEntryKey = SetupResult.ManualEntryKey,
                    QrCodeSetupImageUrl = SetupResult.QrCodeSetupImageUrl,
                    QrString = SetupResult.QrString
                }
            };
        }
        public bool Verify(string googlePin, string accountSecretKey)
        {
            TwoFactorAuthenticator Authenticator = new TwoFactorAuthenticator();
            return Authenticator.ValidateTwoFactorPIN(accountSecretKey, googlePin);
        }

        private string GenrateSecretKey()
        {
            int length = 52;
            const string valid = "ABCDEFGHJKMNPQRSTUVWXYZ234567";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
