using IARA.DomainModels.DTOModels.CommercialFishingRegister;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class WaterInspectionFishingGearDTO : FishingGearDTO
    {
        public bool IsTaken { get; set; }
        public bool IsStored { get; set; }
        public string StorageLocation { get; set; }
    }
}
