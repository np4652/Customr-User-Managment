namespace UserManagement.Domain.Interfaces
{
    public interface IPasswordManager
    {
        Task<string> HashPassword(string password, out string salt);
        Task<bool> VerifyPassword(string password, string hashedPassword, string salt);
    }
}
