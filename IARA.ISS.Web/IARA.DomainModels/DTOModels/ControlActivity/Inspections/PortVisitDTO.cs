using System;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class PortVisitDTO
    {
        public int? PortId { get; set; }
        public string PortName { get; set; }
        public int? PortCountryId { get; set; }
        public DateTime? VisitDate { get; set; }
        public bool IsActive { get; set; }
    }
}
