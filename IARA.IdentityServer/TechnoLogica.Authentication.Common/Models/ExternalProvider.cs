using System;
using System.Collections.Generic;
using System.Text;

namespace TechnoLogica.Authentication.Common
{
    public class ExternalProvider
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }
        public string PartialView { get; set; }
    }
}
