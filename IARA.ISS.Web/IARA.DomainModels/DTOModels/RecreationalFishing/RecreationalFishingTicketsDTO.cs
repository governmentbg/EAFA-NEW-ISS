using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketsDTO
    {
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<RecreationalFishingTicketDTO> Tickets { get; set; }

        public int? AssociationId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasPaymentData { get; set; }

        [RequiredIf(nameof(HasPaymentData), "msgRequired", typeof(ErrorResources), true)]
        public PaymentDataDTO PaymentData { get; set; }
    }
}
