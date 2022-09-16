using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.FishingTickets
{
    public class TicketValidationDTO
    {
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string TypeCode { get; set; }

        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string PeriodCode { get; set; }

        [RequiredIf(nameof(TypeCode), "msgRequired", typeof(ErrorResources), TicketTypeEnum.ELDER, TicketTypeEnum.STANDARD, TicketTypeEnum.BETWEEN14AND18, TicketTypeEnum.ASSOCIATION, TicketTypeEnum.BETWEEN14AND18ASSOCIATION, TicketTypeEnum.ELDERASSOCIATION)]
        public DateTime? ValidFrom { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string EgnLnch { get; set; }

        //Under14
        public DateTime? ChildDateOfBirth { get; set; }

        //Disability
        public bool TELKIsIndefinite { get; set; }

        public DateTime? TELKValidTo { get; set; }
    }
}
