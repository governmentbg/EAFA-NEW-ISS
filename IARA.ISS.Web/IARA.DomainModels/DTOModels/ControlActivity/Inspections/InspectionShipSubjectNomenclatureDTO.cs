using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionShipSubjectNomenclatureDTO : NomenclatureDTO
    {
        public int EntryId { get; set; }
        public EgnLncDTO EgnLnc { get; set; }
        public string Eik { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool IsLegal { get; set; }
        public InspectedPersonTypeEnum Type { get; set; }
        public InspectionSubjectAddressDTO Address { get; set; }
        public int? CountryId { get; set; }
    }
}
