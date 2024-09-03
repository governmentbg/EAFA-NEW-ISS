using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectedLogBookPageDataDTO
    {
        public int? ShipLogBookPageId { get; set; }

        public string ShipLogBookPageNumber { get; set; }

        public DateTime? ShipPageFillDate { get; set; }

        public int? TransportationLogBookPageId { get; set; }

        public decimal? TransportationLogBookPageNumber { get; set; }

        public DateTime? TransportationPageLoadingDate { get; set; } //the name?

        public int? AdmissionLogBookPageId { get; set; }

        public decimal? AdmissionLogBookPageNumber { get; set; }

        public DateTime? AdmissionPageHandoverDate { get; set; }

        public int? FirstSaleLogBookPageId { get; set; }

        public decimal? FirstSaleLogBookPageNumber { get; set; }

        public DateTime? FirstSalePageSaleDate { get; set; }

        public int? AquacultureLogBookPageId { get; set; }

        public decimal? AquacultureLogBookPageNumber { get; set; }

        public DateTime? AquaculturePageFillingDate { get; set; }

        public List<InspectedDeclarationCatchDTO> InspectionCatchMeasures { get; set; }
    }
}
