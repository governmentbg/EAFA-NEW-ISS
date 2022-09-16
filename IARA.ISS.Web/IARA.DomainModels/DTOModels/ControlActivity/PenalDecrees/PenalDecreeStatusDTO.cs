using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees
{
    public class PenalDecreeStatusDTO
    {
        public int? Id { get; set; }

        public int? StatusId { get; set; }

        public string StatusName { get; set; }

        public DateTime? DateOfChange { get; set; }

        public string Details { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
