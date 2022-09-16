using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.AquacultureFacilities.Deregistration
{
    public abstract class AquacultureDeregistrationBaseRegixDataDTO : BaseRegixChecksDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public override int? ApplicationId { get; set; }
    }
}
