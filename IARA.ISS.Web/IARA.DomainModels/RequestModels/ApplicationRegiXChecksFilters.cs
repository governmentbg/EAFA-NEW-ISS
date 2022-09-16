using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class ApplicationRegiXChecksFilters : BaseRequestModel
    {
        public string ApplicationId { get; set; }

        public int? ApplicationTypeId { get; set; }

        public string WebServiceName { get; set; }

        public DateTime? RequestDateFrom { get; set; }

        public DateTime? RequestDateTo { get; set; }

        public DateTime? ResponseDateFrom { get; set; }

        public DateTime? ResponseDateTo { get; set; }

        public List<string> ErrorLevels { get; set; }
    }
}
