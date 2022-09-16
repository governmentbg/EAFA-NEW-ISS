using System;
using Microsoft.Extensions.Configuration;

namespace IARA.Common.ConfigModels
{
    public class ExternalSystemSettings
    {
        public static ExternalSystemSettings Default { get; private set; }

        public string FluxBaseUrl { get; set; }
        public string FvmsBaseUrl { get; set; }
        public RegixVersions RegixVersion { get; set; }
        public bool UseRegixMockChecks { get; set; } = false;

        public static ExternalSystemSettings ReadSettings(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(ExternalSystemSettings));

            if (section.Exists())
            {
                Default = new ExternalSystemSettings
                {
                    FluxBaseUrl = section.GetValue<string>(nameof(FluxBaseUrl)),
                    FvmsBaseUrl = section.GetValue<string>(nameof(FvmsBaseUrl)),
                    RegixVersion = Enum.Parse<RegixVersions>(section.GetValue<string>(nameof(RegixVersion), RegixVersions.V2.ToString())),
                    UseRegixMockChecks = section.GetValue<bool>(nameof(UseRegixMockChecks), false)
                };
            }
            else
            {
                Default = new ExternalSystemSettings();
            }

            return Default;
        }
    }

    public enum RegixVersions
    {
        V1,
        V2
    }
}
