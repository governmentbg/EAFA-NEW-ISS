using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace TechnoLogica.Authentication.Common
{
    public enum CertificatePlatform
    {
        Windows,
        Linux
    }

    public class CertificateSettings : ValidateSettings
    {
        public CertificatePlatform Platfrom { get; set; }
        public string CertificateFileName { get; set; }
        public string CertificatePassword { get; set; }
        public StoreName CertificateStoreName { get; set; }
        public StoreLocation CertificateStoreLocation { get; set; }
        public X509FindType CertificateX509FindType { get; set; }
        public object CertificateFindValue { get; set; }

        public override void Validate()
        {
            switch (this.Platfrom)
            {
                case CertificatePlatform.Linux:
                    {
                        if (string.IsNullOrEmpty(this.CertificateFileName))
                        {
                            throw new ArgumentException("Option CertificateFileName for CertificateSettings is mandatory when Liux platform is specified!");
                        }
                        break;
                    }
                case CertificatePlatform.Windows:
                    {
                        if (this.CertificateFindValue == null)
                        {
                            throw new ArgumentException("Option CertificateFindValue for CertificateSettings is mandatory when Windows platform is specified!");
                        }
                        break;
                    }
            }
        }

        public X509Certificate2 GetX509Certificate2()
        {
            switch (this.Platfrom)
            {
                case CertificatePlatform.Windows:
                    {
                        return SigningUtils.GetX509Certificate(
                            this.CertificateStoreName,
                            this.CertificateStoreLocation,
                            this.CertificateX509FindType,
                            this.CertificateFindValue);
                    }
                case CertificatePlatform.Linux:
                default:
                    {
                        if (this.CertificateFileName.StartsWith("."))
                        {
                            this.CertificateFileName = this.CertificateFileName.TrimStart('.');
                            this.CertificateFileName = this.CertificateFileName.Replace("/", "\\");

                            if (this.CertificateFileName.Contains('\\'))
                            {
                                string[] filePath = this.CertificateFileName.Split('\\', StringSplitOptions.RemoveEmptyEntries);

                                this.CertificateFileName = Path.Combine(Directory.GetCurrentDirectory(), string.Join('\\', filePath.Take(filePath.Length - 1)), filePath[filePath.Length - 1]);
                            }
                            else
                            {
                                this.CertificateFileName = Path.Combine(Directory.GetCurrentDirectory(), this.CertificateFileName);
                            }
                        }

                        return string.IsNullOrEmpty(this.CertificatePassword)
                            ? new X509Certificate2(this.CertificateFileName)
                            : new X509Certificate2(this.CertificateFileName, this.CertificatePassword);
                    }
            }
        }
    }
}
