
namespace SQLQuery
{
    public class User
    {
        public const string Create = @"INSERT INTO Users(Role,UserName,FirstName,LastName,MobileNo,Email,PasswordHash,PasswordSalt,GAuthRequired,GAuthAccountKey) VALUES (@Role,@UserName,@FirstName,@LastName,@MobileNo,@Email,@PasswordHash,@PasswordSalt,@GAuthRequired,@GAuthAccountKey);
                                       SELECT 1 StatusCode,'User Created successfully' ResponseText";
        public const string Delete = @"";
        public const string GetAll = @"";
        public const string Get = @"";
        public const string GetByUserName = @"SELECT * FROM Users(nolock) WHERE UserName = @UserName";
        public const string SetGAuthAccountKey = @"UPDATE Users SET GAuthRequired = 1, GAuthAccountKey = @GAuthAccountKey WHERE UserName = @UserName;
                                                   SELECT 1 StatusCode,'Set successfully' ResponseText";
        public const string SetGAuthRequired = @"UPDATE Users SET GAuthRequired = @enable WHERE UserName = @UserName;
                                                   SELECT 1 StatusCode,'Change successfully' ResponseText";
    }
}
