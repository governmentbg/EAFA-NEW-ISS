using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TechnoLogica.Authentication.Common
{
    public interface IDataProtectionKeyStoreProvider
    {
        IDataProtectionBuilder AddPersistance(IDataProtectionBuilder builder, IConfiguration configuration, ILoggerFactory loggerFactory);
    }
}
