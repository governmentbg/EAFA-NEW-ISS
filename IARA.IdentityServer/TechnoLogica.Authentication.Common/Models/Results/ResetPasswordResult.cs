using System;
using System.Collections.Generic;
using System.Text;

namespace TechnoLogica.Authentication.Common
{
    public class ResetPasswordResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<ResetPasswordError> Errors { get; set; }
    }
}
