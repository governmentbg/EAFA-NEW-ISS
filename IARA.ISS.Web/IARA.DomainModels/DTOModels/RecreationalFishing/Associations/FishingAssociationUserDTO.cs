using IARA.Common.Resources;
using System.ComponentModel.DataAnnotations;
using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.RecreationalFishing.Associations
{
    public class FishingAssociationUserDTO
    {
        public int? AssociationLegalId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? UserId { get; set; }

        public UserLegalStatusEnum? Status { get; set; }

        public string StatusName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
