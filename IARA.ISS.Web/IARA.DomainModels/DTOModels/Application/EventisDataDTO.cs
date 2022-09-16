using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Application
{
    public class EventisDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string EventisNumber { get; set; }
    }
}
