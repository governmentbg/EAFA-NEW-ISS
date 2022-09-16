namespace IARA.DomainModels.DTOModels.Inspectors
{
    public class InspectorsRegisterDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string EgnLnc { get; set; }
        public string IdentifierType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string InspectorCardNum { get; set; }
        public int? InstitutionId { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }
    }
}
