using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees
{
    public class PenalDecreeResolutionDTO
    {
        public int? Id { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Reasons { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Motives { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Zann { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Zra { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string MaterialEvidence { get; set; }

        public bool? IsActive { get; set; }
    }
}
