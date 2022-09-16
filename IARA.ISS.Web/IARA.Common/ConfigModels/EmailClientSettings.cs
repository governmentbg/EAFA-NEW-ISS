using Microsoft.Extensions.Configuration;

namespace IARA.Common.ConfigModels
{
    public class EmailClientSettings
    {
        public static EmailClientSettings Default { get; private set; }

        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderMail { get; set; }
        public string EmailConfirmationUrl { get; set; }
        public byte EmailTokenDaysValid { get; private set; }
        public string ReportViolationEmail { get; set; }

        public static EmailClientSettings ReadSettings(IConfiguration configuration)
        {
            var emailSettingsSection = configuration.GetSection("EmailClientSettings");

            Default = new EmailClientSettings
            {
                Host = emailSettingsSection.GetValue<string>(nameof(Host)),
                Port = emailSettingsSection.GetValue<int>(nameof(Port)),
                SenderMail = emailSettingsSection.GetValue<string>(nameof(SenderMail)),
                EmailConfirmationUrl = emailSettingsSection.GetValue<string>(nameof(EmailConfirmationUrl)),
                EmailTokenDaysValid = emailSettingsSection.GetValue<byte>(nameof(EmailTokenDaysValid)),
                ReportViolationEmail = emailSettingsSection.GetValue<string>(nameof(ReportViolationEmail))
            };

            return Default;
        }
    }
}
