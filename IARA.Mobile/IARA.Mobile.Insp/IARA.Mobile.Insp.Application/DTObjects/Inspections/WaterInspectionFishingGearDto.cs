namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class WaterInspectionFishingGearDto : FishingGearDto
    {
        public int InspectedFishingGearId { get; set; }
        public bool? IsTaken { get; set; }
        public bool? IsStored { get; set; }
        public string StorageLocation { get; set; }
    }
}
