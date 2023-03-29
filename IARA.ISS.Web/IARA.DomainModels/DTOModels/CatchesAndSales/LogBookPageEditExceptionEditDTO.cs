using System;
using IARA.Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPageEditExceptionEditDTO
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? LogBookTypeId { get; set; }

        public int? LogBookId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? ExceptionActiveFrom { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? ExceptionActiveTo { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? EditPageFrom { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? EditPageTo { get; set; }
    }
}
