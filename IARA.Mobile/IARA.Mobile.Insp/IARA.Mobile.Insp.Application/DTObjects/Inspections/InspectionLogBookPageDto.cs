using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionLogBookPageDto
    {
        public int? Id { get; set; }

        public SubjectRoleEnum? InspectedShipType { get; set; }

        public int? ShipId { get; set; }

        public int? UnregisteredShipId { get; set; }

        public VesselDuringInspectionDto OriginShip { get; set; }

        public DeclarationLogBookType? LogBookType { get; set; }

        public int? LogBookId { get; set; }

        public int? AquacultureId { get; set; }

        public string UnregisteredEntityData { get; set; }

        public DateTime? UnregisteredPageDate { get; set; }

        public string UnregisteredPageNum { get; set; }

        public string UnregisteredLogBookNum { get; set; }

        public int? FirstSaleLogBookPageId { get; set; }

        public int? TransportationLogBookPageId { get; set; }

        public int? AdmissionLogBookPageId { get; set; }

        public int? ShipLogBookPageId { get; set; }

        public int? AquacultureLogBookPageId { get; set; }

        public List<InspectedDeclarationCatchDto> InspectionCatchMeasures { get; set; }

        public string CatchMeasuresText { get; set; }
    }
}
