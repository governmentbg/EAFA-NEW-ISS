using System.Collections.Generic;
using IARA.Security.Enum;

namespace IARA.Security
{
    public class UserSecurityModel
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string ClientId { get; set; }
        public LoginTypesEnum LoginType { get; set; }
        public List<string> Permissions { get; set; }
    }
}
