using IARA.Common.Utils;
using Microsoft.Extensions.Configuration;

namespace TLTTS.Common.ConfigModels
{
    public class CorsSettings
    {
        public static CorsSettings Default { get; private set; }

        public string[] AllowedMethods { get; set; }
        public string[] AllowedOrigins { get; set; }

        public static CorsSettings ReadSettings(IConfiguration configuration)
        {
            var section = configuration.GetSection("CorsSettings");

            Default = new CorsSettings
            {
                AllowedMethods = section.GetSettingsArray<string>(nameof(AllowedMethods)),
                AllowedOrigins = section.GetSettingsArray<string>(nameof(AllowedOrigins))
            };

            return Default;
        }
    }
}
