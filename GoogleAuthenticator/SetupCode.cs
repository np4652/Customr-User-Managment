﻿namespace GoogleAuthenticator
{
    public class SetupCode
    {
        public string Account { get; internal set; }
        public string AccountSecretKey { get; internal set; }
        public string ManualEntryKey { get; set; }
        public string QrCodeSetupImageUrl { get; internal set; }
        public string QrString { get; internal set; }
    }
}
