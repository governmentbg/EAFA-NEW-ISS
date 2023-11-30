using System;
using System.Collections.Generic;
using System.Text;

namespace TechnoLogica.Authentication.Common
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string SubjectId { get; set; }
        public string Name { get; set; }
        public bool? Active { get; set; }
    }
}
