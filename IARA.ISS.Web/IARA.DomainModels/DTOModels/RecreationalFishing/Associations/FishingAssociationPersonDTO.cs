using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.RecreationalFishing.Associations
{
    public class FishingAssociationPersonDTO
    {
        public int? Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixPersonDataDTO Person { get; set; }

        // For front-end
        public bool HasRegixDataDiscrepancy { get; set; }

        // For front-end
        public string FullName { get; set; }

        // For front-end
        public string Role { get; set; }

        // For front-end
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
