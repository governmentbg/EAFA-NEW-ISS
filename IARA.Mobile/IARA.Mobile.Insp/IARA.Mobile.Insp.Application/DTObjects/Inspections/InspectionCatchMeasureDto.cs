using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionCatchMeasureDto
    {
        public int? Id { get; set; }
        public int? CatchInspectionTypeId { get; set; }
        public int? FishId { get; set; }
        public decimal? CatchQuantity { get; set; }
        public decimal? AllowedDeviation { get; set; }
        public int? CatchZoneId { get; set; }
        public bool? IsTaken { get; set; }
        public CatchActionEnum? Action { get; set; }
        public string StorageLocation { get; set; }
        public decimal? UnloadedQuantity { get; set; }
        public decimal? AverageSize { get; set; }
        public int? FishSexId { get; set; }
        public int? CatchCount { get; set; }
        public VesselDuringInspectionDto OriginShip { get; set; }
    }
}
