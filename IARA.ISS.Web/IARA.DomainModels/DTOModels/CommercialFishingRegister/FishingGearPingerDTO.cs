using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class FishingGearPingerDTO
    {
        public int? Id { get; set; }

        public string Number { get; set; }

        public int StatusId { get; set; }

        public NomenclatureDTO SelectedStatus { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public bool IsActive { get; set; }
    }
}
