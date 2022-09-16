using System;

namespace IARA.DomainModels.DTOModels.ApplicationRegiXChecks
{
    public class ApplicationRegixCheckRequestDTO
    {
        public int Id { get; set; }

        public string ApplicationId { get; set; }

        public string ApplicationType { get; set; }

        public string WebServiceName { get; set; }

        public string RemoteAddress { get; set; }

        public DateTime RequestDateTime { get; set; }

        public string ResponseStatus { get; set; }

        public DateTime? ResponseDateTime { get; set; }

        public string ErrorLevel { get; set; }

        public string ErrorDescription { get; set; }

        public bool IsActive { get; set; }
    }
}
