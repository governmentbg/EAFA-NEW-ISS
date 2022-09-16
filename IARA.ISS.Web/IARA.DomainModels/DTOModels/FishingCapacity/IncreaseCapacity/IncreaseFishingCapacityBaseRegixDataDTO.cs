using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.FishingCapacity.IncreaseCapacity
{
    public abstract class IncreaseFishingCapacityBaseRegixDataDTO : BaseRegixChecksDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public override int? ApplicationId { get; set; }
    }
}
