using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TechnoLogica.IdentityServer.Models.Account
{
    public class ResetTokenPasswordModel
    {
        public string Button { get; set; }
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public bool Result { get; set; }
        public string EMail { get; set; }
        public string ResetToken { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string NewPassword { get; set; }
        public string RedirectURI { get; set; }
    }
}
