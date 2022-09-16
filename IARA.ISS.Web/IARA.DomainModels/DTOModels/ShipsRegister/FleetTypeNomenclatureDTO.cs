using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class FleetTypeNomenclatureDTO : NomenclatureDTO
    {
        public bool HasControlCard { get; set; }

        public bool HasFitnessCertificate { get; set; }

        public bool HasFishingCapacity { get; set; }
    }
}
