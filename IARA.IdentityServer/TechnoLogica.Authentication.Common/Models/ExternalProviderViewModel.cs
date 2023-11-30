using System;
using System.Collections.Generic;
using System.Text;

namespace TechnoLogica.Authentication.Common
{
    public class ExternalProviderViewModel
    {
        public string ReturnUrl { get; set; }
        public string AuthenticationScheme { get; set; }
        public string DisplayName { get; set; }
    }
}
