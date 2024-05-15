namespace UserManagment.WebAPI.Modals
{
    public class ConfigTwoFactorReq
    {
        public string AuthCode { get; set; }
        public string AccountSecretKey { get; set; }
    }
}
