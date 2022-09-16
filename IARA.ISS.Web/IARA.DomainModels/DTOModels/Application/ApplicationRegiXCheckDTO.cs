using System;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationRegiXCheckDTO
    {
        public int ID { get; set; }

        public int ApplicationId { get; set; }

        public int ApplicationChangeHistoryId { get; set; }

        public string WebServiceName { get; set; }

        public DateTime RequestDateTime { get; set; }

        public string ResponseStatus { get; set; }

        public DateTime? ResponseDateTime { get; set; }

        public string ErrorLevel { get; set; }

        public string ErrorDescription { get; set; }

        public int Attempts { get; set; }
    }
}
