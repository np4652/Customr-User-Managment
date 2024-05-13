using System.Security.Claims;

namespace UserManagement.Domain.Interfaces
{
    public interface ITokenManager
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        IRefreshTokenModel GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }

    public interface IRefreshTokenModel
    {
        string? RefreshToken { get; set; }
        DateTime RefreshTokenExpirationTime { get; set; }
    }
    public class RefreshTokenModel: IRefreshTokenModel
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationTime { get; set; }
    }

    public class JWTConfig
    {
        public string Secretkey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int DurationInMinutes { get; set; }
        public int RefreshTokenExpiry { get; set; }
    }

    public struct ClaimTypesExtension
    {
        public const string Id = "id";
        public const string Role = "role";
        public const string UserName = "userName";
        public const string RefreshToken = "refreshToken";
        public const string BussinessType = "bussinessType";
    }
}
