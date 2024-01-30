using System;

namespace IARA.Mobile.Pub.Domain.Models
{
    public class JwtToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
