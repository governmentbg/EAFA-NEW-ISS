using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectedDeclarationCatchDTO
    {
        public int? Id { get; set; }
        public int? CatchTypeId { get; set; }
        public int? FishTypeId { get; set; }
        public int? CatchCount { get; set; }
        public decimal? CatchQuantity { get; set; }
        public decimal? UnloadedQuantity { get; set; }
        public int? PresentationId { get; set; }
        public int? CatchZoneId { get; set; }
        public VesselDuringInspectionDTO OriginShip { get; set; }
        public int? LogBookPageId { get; set; }
        public DeclarationLogBookTypeEnum? LogBookType { get; set; }
        public string UnregisteredPageNum { get; set; }
        public DateTime? UnregisteredPageDate { get; set; }
    }
}
