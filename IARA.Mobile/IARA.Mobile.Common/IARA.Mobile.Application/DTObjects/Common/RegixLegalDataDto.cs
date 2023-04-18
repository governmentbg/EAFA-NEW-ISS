namespace IARA.Mobile.Application.DTObjects.Common
{
    public class RegixLegalDataDto
    {
        public int? Id { get; set; }
        public string EIK { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool? IsCustodianOfPropertySameAsApplicant { get; set; }
    }
}
