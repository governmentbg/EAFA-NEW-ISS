using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ScientificFishing
{
    public class ScientificFishingMobileDataDTO
    {
        public List<ScientificFishingPermitMobileDTO> Permits { get; set; }
        public List<ScientificFishingPermitHolderMobileDTO> Holders { get; set; }
        public List<ScientificFishingOutingDTO> Outings { get; set; }
        public List<ScientificFishingOutingCatchDTO> Catches { get; set; }
        public List<ScientificFishingPermitReasonDTO> PermitReasons { get; set; }
    }
}
