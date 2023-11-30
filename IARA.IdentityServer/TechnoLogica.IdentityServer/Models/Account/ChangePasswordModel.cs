using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TechnoLogica.IdentityServer.Models.Account
{
    public class ChangePasswordModel
    {
        public string Button { get; set; }
        public string ClientName { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        public string NewPassword { get; set; }
        public string ReturnUrl { get; set; }
    }
}
