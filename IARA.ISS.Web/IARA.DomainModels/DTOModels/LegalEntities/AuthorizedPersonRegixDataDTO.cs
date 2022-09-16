using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.LegalEntities
{
    public class AuthorizedPersonRegixDataDTO
    {
        public int? Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixPersonDataDTO Person { get; set; }

        // For use on front-end
        public bool HasRegixDataDiscrepancy { get; set; }

        public string FullName { get; set; }

        public string RolesAll { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
