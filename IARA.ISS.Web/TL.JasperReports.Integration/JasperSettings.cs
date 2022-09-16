using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using TL.JasperReports.Integration.Enums;

namespace TL.JasperReports.Integration
{
    public class JasperSettings
    {
        public static JasperSettings Default { get; set; }

        public string Host { get; set; }
        public ushort Port { get; set; }

        private string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value != null ? value.Trim() : value;
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value != null ? value.Trim() : value;
            }
        }

        public ContentType Type { get; set; } = ContentType.XML;
        public bool IsProVersion { get; set; } = false;

        public string Scheme { get; set; } = "http";

        private string baseJasperServerPath;
        public string BaseJasperServerPath
        {
            get
            {
                if (string.IsNullOrEmpty(baseJasperServerPath))
                {
                    return $"/jasperserver{(IsProVersion ? "pro" : "")}";
                }
                else
                {
                    return baseJasperServerPath;
                }
            }
            set
            {
                baseJasperServerPath = value;
            }
        }

        internal string GetBasicAuth()
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                string token = $"{Username}:{Password}";
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            }
            else
            {
                return null;
            }
        }

        internal static void LoadSettings(IConfiguration configuration)
        {
            if (Default == null)
            {
                var section = configuration.GetSection(nameof(JasperSettings));

                Default = new JasperSettings
                {
                    BaseJasperServerPath = section.GetValue<string>(nameof(BaseJasperServerPath)),
                    IsProVersion = section.GetValue<bool>(nameof(IsProVersion), false),
                    Password = section.GetValue<string>(nameof(Password)),
                    Username = section.GetValue<string>(nameof(Username)),
                    Host = section.GetValue<string>(nameof(Host)),
                    Port = section.GetValue<ushort>(nameof(Port), 8080),
                    Scheme = section.GetValue<string>(nameof(Scheme), "http")
                };
            }
        }
    }
}
