using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechnoLogica.IdentityServer.Models.Account
{
    public class ExternalLoginErrorViewModel
    {
        public string SchemeDisplayName { get; set; }

        public List<string> Messages { get; set; }

        public List<string> HiddenMessages { get; set; }
    }
}
