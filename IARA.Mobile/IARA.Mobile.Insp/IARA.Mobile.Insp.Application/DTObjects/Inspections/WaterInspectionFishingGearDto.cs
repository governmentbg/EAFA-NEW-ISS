namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class WaterInspectionFishingGearDto : FishingGearDto
    {
        public bool IsStored { get; set; }
        public bool IsTaken { get; set; }
        public string StorageLocation { get; set; }
    }
}
