using System;
using System.Collections.Generic;
using System.Text;

namespace TechnoLogica.Authentication.Common
{
    public class UserRegistrationResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<UserRegistrationError> Errors { get; set; }
    }
}
