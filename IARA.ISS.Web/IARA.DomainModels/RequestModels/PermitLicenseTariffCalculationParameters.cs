using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;

namespace IARA.DomainModels.RequestModels
{
    public class PermitLicenseTariffCalculationParameters
    {
        public int ApplicationId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public PageCodeEnum? PageCode { get; set; }

        public int? ShipId { get; set; }

        public int? WaterTypeId { get; set; }

        public List<int> AquaticOrganismTypeIds { get; set; }

        public List<FishingGearDTO> FishingGears { get; set; }

        public int? PoundNetId { get; set; }
    }
}
