using QRCoder;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace GoogleAuthenticator
{
    public class TwoFactorAuthenticator
    {
        public static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public TimeSpan DefaultClockDriftTolerance { get; set; }

        public bool UseManagedSha1Algorithm { get; set; }

        public bool TryUnmanagedAlgorithmOnFailure { get; set; }

        public TwoFactorAuthenticator() : this(useManagedSha1: true, useUnmanagedOnFail: true)
        {
        }

        public TwoFactorAuthenticator(bool useManagedSha1, bool useUnmanagedOnFail)
        {
            DefaultClockDriftTolerance = TimeSpan.FromMinutes(5.0);
            UseManagedSha1Algorithm = useManagedSha1;
            TryUnmanagedAlgorithmOnFailure = useUnmanagedOnFail;
        }

        public SetupCode GenerateSetupCode(string issuer, string accountTitle, string accountSecretKey)
        {
            if (accountTitle == null)
            {
                throw new NullReferenceException("Account Title is null");
            }
            accountTitle = accountTitle.Replace(" ", "");
            string manualEntryKey = EncodeAccountSecretKey(accountSecretKey);
            string qrString = $"otpauth://totp/{accountTitle}?secret={manualEntryKey}&issuer={UrlEncode(issuer)}";
            return new SetupCode
            {
                Account = accountTitle,
                AccountSecretKey = accountSecretKey,
                ManualEntryKey = manualEntryKey,
                QrString = qrString,
                QrCodeSetupImageUrl = StringToQR(qrString),
            };
        }

        private string StringToQR(string str)
        {
            using (QRCodeGenerator qRCodeGenerator = new QRCodeGenerator())
            {
                QRCodeData QCD = qRCodeGenerator.CreateQrCode(str, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qRCode = new QRCode(QCD))
                {
                    return "data:image/png;base64," + Convert.ToBase64String(BitmapToBytesCode(qRCode.GetGraphic(20)));
                }
            }
        }

        private static byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private string UrlEncode(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            foreach (char c in value)
            {
                if (text.IndexOf(c) != -1)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%" + $"{(int)c:X2}");
                }
            }

            return stringBuilder.ToString().Replace(" ", "%20");
        }

        private string EncodeAccountSecretKey(string accountSecretKey)
        {
            return Base32Encode(Encoding.UTF8.GetBytes(accountSecretKey));
        }

        private string Base32Encode(byte[] data)
        {
            int num = 8;
            int num2 = 5;
            char[] array = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            StringBuilder stringBuilder = new StringBuilder((data.Length + 7) * num / num2);
            while (num3 < data.Length)
            {
                int num6 = ((data[num3] >= 0) ? data[num3] : (data[num3] + 256));
                if (num4 > num - num2)
                {
                    int num7 = ((num3 + 1 < data.Length) ? ((data[num3 + 1] >= 0) ? data[num3 + 1] : (data[num3 + 1] + 256)) : 0);
                    num5 = num6 & (255 >> num4);
                    num4 = (num4 + num2) % num;
                    num5 <<= num4;
                    num5 |= num7 >> num - num4;
                    num3++;
                }
                else
                {
                    num5 = (num6 >> num - (num4 + num2)) & 0x1F;
                    num4 = (num4 + num2) % num;
                    if (num4 == 0)
                    {
                        num3++;
                    }
                }

                stringBuilder.Append(array[num5]);
            }

            return stringBuilder.ToString();
        }

        public string GeneratePINAtInterval(string accountSecretKey, long counter, int digits = 6)
        {
            return GenerateHashedCode(accountSecretKey, counter, digits);
        }

        internal string GenerateHashedCode(string secret, long iterationNumber, int digits = 6)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(secret);
            return GenerateHashedCode(bytes, iterationNumber, digits);
        }

        internal string GenerateHashedCode(byte[] key, long iterationNumber, int digits = 6)
        {
            byte[] bytes = BitConverter.GetBytes(iterationNumber);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            HMACSHA1 hMACSha1Algorithm = getHMACSha1Algorithm(key);
            byte[] array = hMACSha1Algorithm.ComputeHash(bytes);
            int num = array[array.Length - 1] & 0xF;
            int num2 = ((array[num] & 0x7F) << 24) | (array[num + 1] << 16) | (array[num + 2] << 8) | array[num + 3];
            return (num2 % (int)Math.Pow(10.0, digits)).ToString(new string('0', digits));
        }

        private long GetCurrentCounter()
        {
            return GetCurrentCounter(DateTime.UtcNow, _epoch, 30);
        }

        private long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep)
        {
            return (long)(now - epoch).TotalSeconds / timeStep;
        }

        private HMACSHA1 getHMACSha1Algorithm(byte[] key)
        {
            try
            {
                return new HMACSHA1(key, UseManagedSha1Algorithm);
            }
            catch (InvalidOperationException ex2)
            {
                if (UseManagedSha1Algorithm && TryUnmanagedAlgorithmOnFailure)
                {
                    try
                    {
                        return new HMACSHA1(key, useManagedSha1: false);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw ex;
                    }
                }

                throw ex2;
            }
        }

        public bool ValidateTwoFactorPIN(string accountSecretKey, string twoFactorCodeFromClient)
        {
            return ValidateTwoFactorPIN(accountSecretKey, twoFactorCodeFromClient, DefaultClockDriftTolerance);
        }

        public bool ValidateTwoFactorPIN(string accountSecretKey, string twoFactorCodeFromClient, TimeSpan timeTolerance)
        {
            string[] currentPINs = GetCurrentPINs(accountSecretKey, timeTolerance);
            return currentPINs.Any((string c) => c == twoFactorCodeFromClient);
        }

        public string GetCurrentPIN(string accountSecretKey)
        {
            return GeneratePINAtInterval(accountSecretKey, GetCurrentCounter());
        }

        public string GetCurrentPIN(string accountSecretKey, DateTime now)
        {
            return GeneratePINAtInterval(accountSecretKey, GetCurrentCounter(now, _epoch, 30));
        }

        public string[] GetCurrentPINs(string accountSecretKey)
        {
            return GetCurrentPINs(accountSecretKey, DefaultClockDriftTolerance);
        }

        public string[] GetCurrentPINs(string accountSecretKey, TimeSpan timeTolerance)
        {
            List<string> list = new List<string>();
            long currentCounter = GetCurrentCounter();
            int num = 0;
            if (timeTolerance.TotalSeconds > 30.0)
            {
                num = Convert.ToInt32(timeTolerance.TotalSeconds / 30.0);
            }

            long num2 = currentCounter - num;
            long num3 = currentCounter + num;
            for (long num4 = num2; num4 <= num3; num4++)
            {
                list.Add(GeneratePINAtInterval(accountSecretKey, num4));
            }

            return list.ToArray();
        }
    }
}
