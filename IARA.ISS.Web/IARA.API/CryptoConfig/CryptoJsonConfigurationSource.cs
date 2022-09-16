using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace IARA.Web
{
    public class CryptoJsonConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CryptoJsonConfigurationProvider(this);
        }
    }
}
