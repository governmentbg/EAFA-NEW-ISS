using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ScientificFishing
{
    public class ScientificFishingPermitRegixDataDTO : ScientificFishingPermitBaseRegixDataDTO
    {
        public List<ScientificFishingPermitHolderRegixDataDTO> Holders { get; set; }
    }
}
