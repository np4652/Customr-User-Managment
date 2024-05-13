using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.Base
{
    public enum ResponseStatus
    {
        Fail = -1,
        All = 0,
        Success = 1,
        Pending = 2,
        Failed = 3,
        [Display(Name = "Token Expired")]
        TokenExpired = 4,
        [Display(Name = "Insufficient Balance")]
        InsufficientBalance = 5,
        [Display(Name = "Inititate Payout")]
        InititatePayout = 6,
        [Display(Name = "Payout Success")]
        PayoutSuccess = 7,
        [Display(Name = "Payout Failed")]
        PayoutFailed = 8,
        [Display(Name = "Payout Status Recheck")]
        PayoutStatusReCheck = 9,
    }
}
