using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class PermitNomenclatureDTO : NomenclatureDTO
    {
        public CommercialFishingTypesEnum Type { get; set; }

        public string CaptainName { get; set; }

        public string ShipOwnerName { get; set; }

        public int? ShipOwnerPersonId { get; set; }

        public int? ShipOwnerLegalId { get; set; }

        public int? CaptainId { get; set; }
    }
}
