using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class CatchDto : IActive
    {
        public int Id { get; set; }
        public int ShipUid { get; set; }
        public int LogBookId { get; set; }
        public string PageNumber { get; set; }
        public int FishId { get; set; }
        public int? CatchTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnloadedQuantity { get; set; }
        public int? TurbotSizeGroupId { get; set; }
        public int? CatchZoneId { get; set; }
        public int FishingGearPermitId { get; set; }
        public bool IsActive { get; set; }
    }
}
