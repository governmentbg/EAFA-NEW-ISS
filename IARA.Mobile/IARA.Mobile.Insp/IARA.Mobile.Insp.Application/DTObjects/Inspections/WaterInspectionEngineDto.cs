namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class WaterInspectionEngineDto
    {
        public int? Id { get; set; }
        public string Model { get; set; }
        public decimal? Power { get; set; }
        public string Type { get; set; }
        public int? TotalCount { get; set; }
        public bool IsStored { get; set; }
        public bool IsTaken { get; set; }
        public string StorageLocation { get; set; }
        public string EngineDescription { get; set; }
    }
}
