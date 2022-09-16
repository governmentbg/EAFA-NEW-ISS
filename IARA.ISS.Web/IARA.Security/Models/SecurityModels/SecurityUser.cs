using System;

namespace IARA.Security.SecurityModels
{
    public class SecurityUser
    {
        public string Email { get; set; }
        public int Id { get; set; }
        public string UserName { get; set; }
        public short AccessFailedCount { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public bool HasEauthLogin { get; set; }
        public bool HasUserPassLogin { get; set; }
        public bool IsInternalUser { get; set; }
        public int PersonId { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockEndDateTime { get; set; }
        public bool UserMustChangePassword { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
