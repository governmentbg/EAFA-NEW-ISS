using System.Collections.Generic;
using IARA.Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionEmailDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? InspectionId { get; set; }

        public List<InspectedEntityEmailDTO> InspectedEntityEmails { get; set; }
    }
}
