using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Application.DTObjects.Profile.API
{
    public class RegixLegalDataApiDto
    {
        public int Id { get; set; }
        public string EIK { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CustodianOfPropertyFirstName { get; set; }
        public string CustodianOfPropertyMiddleName { get; set; }
        public string CustodianOfPropertyLastName { get; set; }
        public string CustodianOfPropertyEGN { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public NomenclatureDto SelectedLegal { get; set; }
        public NomenclatureDto SelectedLegalRole { get; set; }
    }
}
