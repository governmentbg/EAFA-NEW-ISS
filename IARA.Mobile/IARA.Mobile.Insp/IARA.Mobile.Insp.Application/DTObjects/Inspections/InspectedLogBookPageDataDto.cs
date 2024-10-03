using System;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectedLogBookPageDataDto
    {
        public int? ShipLogBookPageId { get; set; }

        public string ShipLogBookPageNumber { get; set; }

        public DateTime? ShipPageFillDate { get; set; }

        public int? TransportationLogBookPageId { get; set; }

        public decimal? TransportationLogBookPageNumber { get; set; }

        public DateTime? TransportationPageLoadingDate { get; set; }

        public int? AdmissionLogBookPageId { get; set; }

        public decimal? AdmissionLogBookPageNumber { get; set; }

        public DateTime? AdmissionPageHandoverDate { get; set; }

        public int? FirstSaleLogBookPageId { get; set; }

        public decimal? FirstSaleLogBookPageNumber { get; set; }

        public DateTime? FirstSalePageSaleDate { get; set; }

        public int? AquacultureLogBookPageId { get; set; }

        public decimal? AquacultureLogBookPageNumber { get; set; }

        public DateTime? AquaculturePageFillingDate { get; set; }

        public List<InspectedDeclarationCatchDto> InspectionCatchMeasures { get; set; }
    }
}
