using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPermitLicenseNomenclatureDTO : NomenclatureDTO
    {
        public int PermitLicenseId { get; set; }

        public int LogBookId { get; set; }

        public string PermitLicenseNumber { get; set; }

        public string PermitLicenseName { get; set; }

        public WaterTypesEnum PermitLicenseWaterType { get; set; }

        /// <summary>
        /// For UI
        /// </summary>
        public string PermitLicenseWaterTypeName { get; set; }
    }
}
