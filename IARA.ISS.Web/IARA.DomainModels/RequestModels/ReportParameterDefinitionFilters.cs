using System;

namespace IARA.DomainModels.RequestModels
{
    public class ReportParameterDefinitionFilters : BaseRequestModel
    {
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
