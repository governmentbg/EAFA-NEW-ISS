using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechnoLogica.IdentityServer.Models.Account
{
    public class InvalidSignatureViewModel
    {
        public InvalidSignatureViewModel()
        {
            this.Messages = new List<string>();
            this.HiddenMessages = new List<string>();
        }

        public List<string> Messages { get; set; }

        public List<string> HiddenMessages { get; set; }

        public bool BoldLastMessage { get; set; }
    }
}
