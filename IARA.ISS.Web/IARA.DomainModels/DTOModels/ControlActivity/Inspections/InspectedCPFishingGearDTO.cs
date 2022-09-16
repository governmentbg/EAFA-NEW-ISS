namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectedCPFishingGearDTO
    {
        public int Id { get; set; }
        public int FishingGearId { get; set; }
        public int GearCount { get; set; }
        public decimal Length { get; set; }
        public string Description { get; set; }
        public bool IsStored { get; set; }
        public bool IsTaken { get; set; }
    }
}
