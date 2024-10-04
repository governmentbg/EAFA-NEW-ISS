using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionFirstSaleDto : InspectionEditDto
    {
        public string SubjectName { get; set; }
        public string SubjectAddress { get; set; }
        public string RepresentativeComment { get; set; }
        public List<InspectionLogBookPageDto> InspectionLogBookPages { get; set; }
    }
}
