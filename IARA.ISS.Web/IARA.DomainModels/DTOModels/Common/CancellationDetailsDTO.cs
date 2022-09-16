using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Common
{
    public class CancellationDetailsDTO
    {
        public int Id { get; set; }

        public int ReasonId { get; set; }

        public DateTime Date { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string IssueOrderNum { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
