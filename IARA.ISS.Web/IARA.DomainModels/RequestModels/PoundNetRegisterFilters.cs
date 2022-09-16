using System;

namespace IARA.DomainModels.RequestModels
{
    public class PoundNetRegisterFilters : BaseRequestModel
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public int? MuncipalityId { get; set; }
        public int? SeasonTypeId { get; set; }
        public int? CategoryTypeId { get; set; }
        public DateTime? RegisteredDateFrom { get; set; }
        public DateTime? RegisteredDateTo { get; set; }
        public int? StatusId { get; set; }
    }
}
