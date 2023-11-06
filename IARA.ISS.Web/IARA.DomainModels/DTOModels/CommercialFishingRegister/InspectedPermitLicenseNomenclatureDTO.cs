using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class InspectedPermitLicenseNomenclatureDTO : NomenclatureDTO
    {
        public int? Year { get; set; }

        public string UnregisteredPermitLicenseNum { get; set; }

        public string InspectionReportNum { get; set; }
    }
}
