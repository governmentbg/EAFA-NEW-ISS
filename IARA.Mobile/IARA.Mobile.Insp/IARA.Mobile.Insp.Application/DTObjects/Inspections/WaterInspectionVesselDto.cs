namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class WaterInspectionVesselDto
    {
        public int? Id { get; set; }
        public int? VesselTypeId { get; set; }
        public string Number { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public string Color { get; set; }
        public int? TotalCount { get; set; }
        public bool IsStored { get; set; }
        public bool IsTaken { get; set; }
        public string StorageLocation { get; set; }
    }
}
