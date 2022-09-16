using Microsoft.Extensions.Configuration;

namespace IARA.Common.ConfigModels
{
    public class StartupSettings
    {
        public static StartupSettings Default { get; set; }
        public string BasePath { get; set; } = string.Empty;

        public BackgroundTasksSettings BackgroundTasks { get; set; }

        public static StartupSettings ReadSettings(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(StartupSettings));

            Default = new StartupSettings
            {
                BasePath = section.GetValue<string>(nameof(BasePath), string.Empty),
                BackgroundTasks = BackgroundTasksSettings.ReadSettings(section)
            };

            return Default;
        }
    }

    public class BackgroundTasksSettings
    {
        public BackgroundTasksSettings()
        {
            this.Enabled = true;
            this.SendMobileNotifications = true;
            this.MontlyCrossChecks = true;
            this.DailyCrossChecks = true;
            this.ACDRReporting = true;
            this.InactivatePastTickets = true;
            this.UnlockUsers = true;
        }

        public bool Enabled { get; set; }
        public bool SendMobileNotifications { get; set; }
        public bool UnlockUsers { get; set; }
        public bool InactivatePastTickets { get; set; }
        public bool ACDRReporting { get; set; }
        public bool DailyCrossChecks { get; set; }
        public bool MontlyCrossChecks { get; set; }
        public bool WeeklyCrossChecks { get; set; }
        public bool PermitsSuspensionFlagUpdate { get; set; }
        public bool PermitLicensesSuspensionFlagUpdate { get; set; }

        public static BackgroundTasksSettings ReadSettings(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(BackgroundTasksSettings));

            if (section.Exists())
            {
                return new BackgroundTasksSettings
                {
                    Enabled = section.GetValue<bool>(nameof(Enabled), true),
                    ACDRReporting = section.GetValue<bool>(nameof(ACDRReporting), true),
                    DailyCrossChecks = section.GetValue<bool>(nameof(DailyCrossChecks), true),
                    InactivatePastTickets = section.GetValue<bool>(nameof(InactivatePastTickets), true),
                    MontlyCrossChecks = section.GetValue<bool>(nameof(MontlyCrossChecks), true),
                    SendMobileNotifications = section.GetValue<bool>(nameof(SendMobileNotifications), true),
                    UnlockUsers = section.GetValue<bool>(nameof(UnlockUsers), true),
                    WeeklyCrossChecks = section.GetValue<bool>(nameof(WeeklyCrossChecks), true),
                    PermitsSuspensionFlagUpdate = section.GetValue<bool>(nameof(PermitsSuspensionFlagUpdate), true),
                    PermitLicensesSuspensionFlagUpdate = section.GetValue<bool>(nameof(PermitLicensesSuspensionFlagUpdate), true)
                };
            }
            else
            {
                return new BackgroundTasksSettings();
            }
        }
    }
}
