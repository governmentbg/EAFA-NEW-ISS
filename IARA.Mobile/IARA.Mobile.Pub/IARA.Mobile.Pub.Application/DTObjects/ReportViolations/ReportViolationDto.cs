using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using System;

namespace IARA.Mobile.Pub.Application.DTObjects.ReportViolations
{
    public class ReportViolationDto
    {
        public NomenclatureDto SignalType { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public DateTime? Date { get; set; }
        public LocationDto Location { get; set; }
    }
}
