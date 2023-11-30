using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TechnoLogica.Authentication.Common;

namespace TechnoLogica.Authentication.EAuth
{
    public class EAuthSettings : ValidateSettings
    {
        public CertificateSettings ClientSystemCertificate { get; set; }
        public CertificateSettings EAuthSystemCertificate { get; set; }        

        public string SystemProviderOID { get; set; }
        public string RequestServiceOID { get; set; }
        public string InformationSystemName { get; set; }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(SystemProviderOID))
            {
                throw new ArgumentException("Option SystemProviderOID is mandatory!");
            }
            if (string.IsNullOrEmpty(RequestServiceOID))
            {
                throw new ArgumentException("Option RequestServiceOID is mandatory!");
            }
            if (string.IsNullOrEmpty(InformationSystemName))
            {
                throw new ArgumentException("Option InformationSystemName is mandatory!");
            }
            if (ClientSystemCertificate == null)
            {
                throw new ArgumentException("Option ClientSystemCertificate is mandatory!");
            }
            if (EAuthSystemCertificate == null)
            {
                throw new ArgumentException("Option EAuthSystemCertificate is mandatory!");
            }
            ClientSystemCertificate.Validate();
            EAuthSystemCertificate.Validate();
        }
    }
}
