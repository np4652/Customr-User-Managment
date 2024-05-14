using System.ComponentModel.DataAnnotations;

namespace UserManagement.Entities
{
    public enum ResponseStatus
    {
        Fail = -1,
        All = 0,
        Success = 1,
        Pending = 2,
        Failed = 3,
        GAuthRequired = 4,
        [Display(Name = "Token Expired")]
        TokenExpired = 5,
    }
}
