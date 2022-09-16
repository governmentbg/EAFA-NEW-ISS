namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionObservationToolDTO
    {
        public int ObservationToolId { get; set; } //NobservationToolsId
        public string Description { get; set; }
        public bool IsOnBoard { get; set; }
        public bool IsActive { get; set; }
    }
}
