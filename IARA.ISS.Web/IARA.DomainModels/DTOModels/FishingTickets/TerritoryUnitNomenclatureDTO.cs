using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.FishingTickets
{
    public class TerritoryUnitNomenclatureDTO : NomenclatureDTO
    {
        public string Address { get; set; }

        public string Phone { get; set; }

        public string WorkingTime { get; set; }

        public string DeliveryTerritoryUniitMessage { get; set; }
    }
}
