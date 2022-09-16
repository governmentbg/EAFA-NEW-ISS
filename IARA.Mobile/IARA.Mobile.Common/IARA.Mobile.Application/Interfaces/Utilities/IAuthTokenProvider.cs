using System;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IAuthTokenProvider
    {
        string Token { get; set; }
        string RefreshToken { get; set; }
        DateTime AccessTokenExpiration { get; set; }
        bool IsUnregisteredEAuthUser { get; set; }
        void Clear();
    }
}
