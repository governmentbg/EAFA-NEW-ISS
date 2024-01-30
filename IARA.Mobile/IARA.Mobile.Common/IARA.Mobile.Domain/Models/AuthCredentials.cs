using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.Mobile.Domain.Models
{
    public class AuthCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
