namespace Usermanagment.WebApp.Models
{
    public class TwoFactorSetupVM
    {
        public string Account { get; set; }
        public string AccountSecretKey { get; set; }
        public string ManualEntryKey { get; set; }
        public string QrCodeSetupImageUrl { get; set; }
        public string QrString { get; set; }
    }
}
