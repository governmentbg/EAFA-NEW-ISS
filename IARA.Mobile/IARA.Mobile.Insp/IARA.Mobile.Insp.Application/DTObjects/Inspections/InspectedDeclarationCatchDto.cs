using IARA.Mobile.Insp.Domain.Enums;
using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectedDeclarationCatchDto
    {
        public int? Id { get; set; }
        public int? InspectionLogBookPageId { get; set; }
        public int? CatchTypeId { get; set; }
        public int? FishTypeId { get; set; }
        public int? CatchCount { get; set; }
        public decimal? CatchQuantity { get; set; }
        public decimal? UnloadedQuantity { get; set; }
        public int? PresentationId { get; set; }
        public bool? Undersized { get; set; }
        public int? CatchZoneId { get; set; }
        public int? TurbotSizeGroupId { get; set; }
        public VesselDuringInspectionDto OriginShip { get; set; }
        public int? AquacultureId { get; set; }
        public string UnregisteredEntityData { get; set; }
        public int? LogBookPageId { get; set; }
        public DeclarationLogBookType? LogBookType { get; set; }
        public string UnregisteredPageNum { get; set; }
        public string UnregisteredLogBookNum { get; set; }
        public DateTime? UnregisteredPageDate { get; set; }
    }
}
