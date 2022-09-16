using System;
using IARA.Common.Attributes;

namespace IARA.DomainModels.RequestModels
{
    public class NomenclaturesFilters : BaseRequestModel
    {
        [Ignore]
        public int TableId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public DateTime? ValidityDateFrom { get; set; }

        public DateTime? ValidityDateTo { get; set; }
    }
}
