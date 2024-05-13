using System.Security.Cryptography;
using System.Text;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Infrastructure.Services
{
    public class PasswordManager : IPasswordManager
    {
        public async Task<bool> VerifyPassword(string password, string hashedPassword, string salt)
        {
            // Convert salt and password to bytes
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Combine password and salt
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

            // Hash the combined bytes
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                string hashedInputPassword = Convert.ToBase64String(hashBytes);
                return await Task.FromResult(hashedInputPassword == hashedPassword);
            }
        }

        public Task<string> HashPassword(string password, out string salt)
        {
            // Generate a random salt
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);
            // Combine password and salt
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

            // Hash the combined bytes
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Task.FromResult(Convert.ToBase64String(hashBytes));
            }
        }
    }
}
