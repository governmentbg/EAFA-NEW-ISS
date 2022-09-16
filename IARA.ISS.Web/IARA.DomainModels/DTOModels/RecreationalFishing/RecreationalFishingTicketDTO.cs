using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketDTO : RecreationalFishingTicketBaseRegixDataDTO
    {
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string TicketNum { get; set; }

        public string DuplicateOfTicketNum { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? TypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? PeriodId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? Price { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public DateTime IssuedOn { get; set; }

        public FileInfoDTO PersonPhoto { get; set; }

        public RecreationalFishingMembershipCardDTO MembershipCard { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comment { get; set; }

        public List<RecreationalFishingTicketDuplicateTableDTO> TicketDuplicates { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasUserConfirmed { get; set; }

        public List<FileInfoDTO> Files { get; set; }

        public FileInfoDTO DeclarationFile { get; set; }
    }
}
