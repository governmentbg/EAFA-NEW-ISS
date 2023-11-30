using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TechnoLogica.IdentityServer.Models.Account
{
    public class ResetPasswordModel
    {
        public string Button { get; set; }
        public string ClientName { get; set; }
        [Required]
        public string EMail { get; set; }
        public bool Result { get; set; }
        public string ReturnUrl { get; set; }
    }
}
