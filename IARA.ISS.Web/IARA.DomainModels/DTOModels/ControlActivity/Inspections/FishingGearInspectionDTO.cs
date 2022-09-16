namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class FishingGearInspectionDTO
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int PermitId { get; set; }
        public int TypeId { get; set; }
        public int Count { get; set; }
        public decimal? Length { get; set; }
        public decimal? Height { get; set; }
        public decimal? NetEyeSize { get; set; }
        public int? HookCount { get; set; }
        public string Description { get; set; }
        public int? TowelLength { get; set; }
        public int? HouseLength { get; set; }
        public int? HouseWidth { get; set; }
        public decimal? CordThickness { get; set; }
        public bool IsActive { get; set; }
    }
}
