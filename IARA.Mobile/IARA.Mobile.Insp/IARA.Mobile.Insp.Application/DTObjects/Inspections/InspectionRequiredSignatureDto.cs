namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionRequiredSignatureDto
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public PersonSignatureType SignatureType { get; set; }
    }
    public enum PersonSignatureType
    {
        Inspector = 0,
        InspectedPerson = 1
    }
}
