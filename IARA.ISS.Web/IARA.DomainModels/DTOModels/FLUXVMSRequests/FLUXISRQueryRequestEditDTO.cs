using IARA.Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace IARA.DomainModels.DTOModels.FLUXVMSRequests
{
    public class FLUXISRQueryRequestEditDTO
    {
        public DateTime? SubmittedDateTime { get; set; }


        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string InspectionType { get; set; } //TODO enum


        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? InspectionStart { get; set; }


        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? InspectionEnd { get; set; }
    }
}
