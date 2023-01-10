namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectorDto : UnregisteredPersonDto
    {
        public string CardNum { get; set; }
        public string TerritoryCode { get; set; }
        public int? InspectorId { get; set; }
        public int? UserId { get; set; }
        public int? UnregisteredPersonId { get; set; }
        public bool IsNotRegistered { get; set; }
        public int? InstitutionId { get; set; }
        public int? InspectionSequenceNumber { get; set; }
    }
}
