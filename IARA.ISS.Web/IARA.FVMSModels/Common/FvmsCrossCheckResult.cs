using System.ComponentModel.DataAnnotations;

namespace IARA.FVMSModels.Common
{
    public class FvmsCrossCheckResult
    {
        [Required]
        public int? CrossCheckId { get; set; }

        [Required]
        [StringLength(50)]
        public string RecordUID { get; set; }

        [Required]
        [StringLength(4000)]
        public string ErrorDescription { get; set; }
    }
}
