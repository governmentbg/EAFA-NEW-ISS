namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectorDTO : UnregisteredPersonDTO
    {
        public string CardNum { get; set; }
        public int? InspectorId { get; set; }
        public int? UserId { get; set; }
        public int? UnregisteredPersonId { get; set; }
        public bool IsNotRegistered { get; set; }
        public int? InstitutionId { get; set; }
        public string Institution { get; set; }
    }
}
