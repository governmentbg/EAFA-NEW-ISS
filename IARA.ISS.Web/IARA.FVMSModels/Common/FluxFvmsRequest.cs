using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.FVMSModels.Common
{
    public class FluxFvmsRequest
    {
        [Required]
        public Guid? RequestUuid { get; set; }

        [Required]
        public DateTime? RequestDateTime { get; set; }

        [Required]
        public bool? IsOutgoing { get; set; }

        [Required]
        [MaxLength(100)]
        public string DomainName { get; set; }

        [Required]
        [MaxLength(500)]
        public string ServiceName { get; set; }

        [Required]
        public string RequestContent { get; set; }
    }
}
