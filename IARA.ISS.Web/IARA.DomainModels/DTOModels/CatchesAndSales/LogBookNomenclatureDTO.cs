
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookNomenclatureDTO : NomenclatureDTO
    {
        public string OwnerName { get; set; }

        public LogBookPagePersonTypesEnum? OwnerType { get; set; }

        public int? LogBookPermitLicenseId { get; set; }    

        public string PermitLicenseNumber { get; set; } 
    }
}
