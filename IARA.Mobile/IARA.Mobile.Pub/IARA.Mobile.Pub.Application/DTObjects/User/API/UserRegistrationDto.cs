using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Pub.Application.DTObjects.User.API
{
    public class UserRegistrationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public EgnLncDto EgnLnc { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool? HasUserPassLogin { get; set; }
        public bool? HasEAuthLogin { get; set; }
        public bool IsInternalUser { get; set; }
        public LoginTypeEnum CurrentLoginType { get; set; }
    }
}
