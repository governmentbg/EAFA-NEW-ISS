using IARA.Common.Utils;
using Microsoft.Extensions.Configuration;

namespace IARA.Common.ConfigModels
{
    public class JwtBearerSettings
    {
        public static JwtBearerSettings Default { get; private set; }

        public string[] Authorities { get; private set; }
        public string[] Audiences { get; private set; }
        public bool ShouldValidateServerCert { get; set; }

        public static JwtBearerSettings ReadSettings(IConfiguration configuration)
        {
            var section = configuration.GetSection("JwtBearerOptions");

            Default = new JwtBearerSettings
            {
                Authorities = section.GetSettingsArray<string>(nameof(Authorities)),
                Audiences = section.GetSettingsArray<string>(nameof(Audiences)),
                ShouldValidateServerCert = section.GetValue<bool>(nameof(ShouldValidateServerCert), true)
            };

            return Default;
        }
    }
}
