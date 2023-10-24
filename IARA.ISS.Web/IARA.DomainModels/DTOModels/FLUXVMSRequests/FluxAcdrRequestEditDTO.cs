using System;
using IARA.Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace IARA.DomainModels.DTOModels.FLUXVMSRequests
{
    public class FluxAcdrRequestEditDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? ToDate { get; set; }
    }
}
