using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class FishingGearInspectionNomenclatureDto : IActive
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
        public decimal? CordThickness { get; set; }
        public int? HouseLength { get; set; }
        public int? HouseWidth { get; set; }
        public int? LineCount { get; set; }
        public decimal? NetNominalLength { get; set; }
        public decimal? NetsInFleetCount { get; set; }
        public string TrawlModel { get; set; }
        public bool IsActive { get; set; }
    }
}
