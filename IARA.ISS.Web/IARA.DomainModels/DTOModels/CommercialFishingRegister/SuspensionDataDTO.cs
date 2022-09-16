using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class SuspensionDataDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? SuspensionTypeId { get; set; }

        public string SuspensionTypeName { get; set; } // to display in table only

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ReasonId { get; set; }

        public string ReasonName { get; set; } // to display in table only

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? EnactmentDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string OrderNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
