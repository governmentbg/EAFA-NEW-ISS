using IARA.Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace IARA.DomainModels.DTOModels.PrintConfigurations
{
    public class PrintConfigurationEditDTO
    {
        public int? Id { get; set; }

        public int? ApplicationTypeId { get; set; }

        public int? TerritoryUnitId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? SignUserId { get; set; }

        public int? SubstituteUserId { get; set; }

        [StringLength(1000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string SubstituteReason { get; set; }
    }
}
