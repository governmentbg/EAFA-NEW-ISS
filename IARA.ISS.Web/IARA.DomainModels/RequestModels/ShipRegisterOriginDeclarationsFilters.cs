using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.RequestModels
{
    public class ShipRegisterOriginDeclarationsFilters : BaseRequestModel
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipUID { get; set; }
    }
}
