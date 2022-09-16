using IARA.Mobile.Insp.Domain.Enums;
using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectedDeclarationCatchDto
    {
        public int? Id { get; set; }
        public int? CatchTypeId { get; set; }
        public int? FishTypeId { get; set; }
        public decimal? CatchQuantity { get; set; }
        public decimal? UnloadedQuantity { get; set; }
        public int? PresentationId { get; set; }
        public int? CatchZoneId { get; set; }
        public VesselDuringInspectionDto OriginShip { get; set; }
        public int? LogBookPageId { get; set; }
        public DeclarationLogBookType? LogBookType { get; set; }
        public string UnregisteredPageNum { get; set; }
        public DateTime? UnregisteredPageDate { get; set; }
    }
}
