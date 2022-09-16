using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.FVMSModels.Common
{
    public class FluxFvmsResponse
    {
        [Required]
        public Guid? RequestUuid { get; set; }

        [Required]
        public Guid? ResponseUuid { get; set; }

        [Required]
        public DateTime? ResponseDateTime { get; set; }

        // "Ok", "Error", "Warning"
        [Required]
        [MaxLength(50)]
        public string ResponseStatus { get; set; }

        public string ResponseContent { get; set; }

        [MaxLength(4000)]
        public string ErrorDescription { get; set; }
    }
}
