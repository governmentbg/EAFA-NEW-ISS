using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.FishingTickets
{
    public class MobileVersionNomenclatureDTO : NomenclatureDTO
    {
        public int Version { get; set; }
        public string OSType { get; set; }
    }
}
