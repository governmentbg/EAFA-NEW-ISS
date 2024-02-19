using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookOwnerNomenclatureDTO : NomenclatureDTO
    {
        public int? PersonId { get; set; }

        public int? LegalId { get; set; }

        public int? BuyerId { get; set; }

        public string OwnerName { get; set; }

        public string LogBookNumber { get; set; }

        public LogBookTypesEnum? LogBookType { get; set; }

        public LogBookPagePersonTypesEnum? OwnerType { get; set; }
    }
}
