using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class PortVisitDto
    {
        public int? PortId { get; set; }
        public string PortName { get; set; }
        public int? PortCountryId { get; set; }
        public DateTime? VisitDate { get; set; }
    }
}
