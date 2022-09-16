using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionCatchMeasureDTO
    {
        public int? Id { get; set; }
        public int? CatchInspectionTypeId { get; set; }
        public int? FishId { get; set; }
        public decimal? CatchQuantity { get; set; }
        public decimal? AllowedDeviation { get; set; }
        public int? CatchZoneId { get; set; }
        public bool? IsTaken { get; set; }
        public CatchActionEnum? Action { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string StorageLocation { get; set; }
        public decimal? UnloadedQuantity { get; set; }
        public decimal? AverageSize { get; set; }
        public int? FishSexId { get; set; }
        public int? CatchCount { get; set; }
        public VesselDuringInspectionDTO OriginShip { get; set; }
    }
}
