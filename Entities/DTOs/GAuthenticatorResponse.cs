namespace Entities.DTOs
{
    public class GAuthenticatorResponse
    {
        public bool Enabled { get; set; }
        public SetupCode Configuration { get; set; }
        public class SetupCode
        {
            public string Account { get; set; }
            public string AccountSecretKey { get; set; }
            public string ManualEntryKey { get; set; }
            public string QrCodeSetupImageUrl { get; set; }
            public string QrString { get; set; }
        }
    }
}
