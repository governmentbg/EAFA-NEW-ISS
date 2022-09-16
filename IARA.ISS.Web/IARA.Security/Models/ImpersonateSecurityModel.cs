using System;

namespace IARA.Security.Models
{
    public class ImpersonateSecurityModel
    {
        public UserSecurityModel User { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
