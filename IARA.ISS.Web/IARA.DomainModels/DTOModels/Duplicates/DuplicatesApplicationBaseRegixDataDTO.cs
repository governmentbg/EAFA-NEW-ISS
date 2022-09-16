using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Duplicates
{
    public class DuplicatesApplicationBaseRegixDataDTO : BaseRegixChecksDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public override int? ApplicationId { get; set; }
    }
}
