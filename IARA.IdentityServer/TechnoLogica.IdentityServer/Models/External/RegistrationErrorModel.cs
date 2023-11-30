using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoLogica.Authentication.Common;

namespace TechnoLogica.IdentityServer.Models.External
{
    public class RegistrationErrorModel
    {
        public IEnumerable<UserRegistrationError> Errors { get; set; }

        public string PostLogoutRedirectUri { get; set; }
    }
}
