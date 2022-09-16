namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class FishingGearPingerInspectionDTO
    {
        public int Id { get; set; }
        public int FishingGearId { get; set; }
        public string Number { get; set; }
        public int StatusId { get; set; }
        public bool IsActive { get; set; }
    }
}
