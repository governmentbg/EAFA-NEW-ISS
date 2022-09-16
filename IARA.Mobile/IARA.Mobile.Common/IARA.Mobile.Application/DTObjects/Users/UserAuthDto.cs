using IARA.Mobile.Domain.Enums;
using System.Collections.Generic;

namespace IARA.Mobile.Application.DTObjects.Users
{
    public class UserAuthDto
    {
        public int Id { get; set; }
        public string EgnLnch { get; set; }
        public IdentifierTypeEnum IdentifierType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool? HasUserPassLogin { get; set; }
        public bool? HasEAuthLogin { get; set; }
        public LoginTypeEnum CurrentLoginType { get; set; }
        public bool UserMustChangePassword { get; set; }
        public bool IsInternalUser { get; set; }
        public List<string> Permissions { get; set; }
    }
}
