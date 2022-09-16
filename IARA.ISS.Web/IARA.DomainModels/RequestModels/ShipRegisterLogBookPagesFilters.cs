using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.RequestModels
{
    public class ShipRegisterLogBookPagesFilters : BaseRequestModel
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipUID { get; set; }
    }
}
