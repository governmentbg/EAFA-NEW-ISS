using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketDeclarationParametersDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string TicketNum { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public TicketTypeEnum? Type { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public TicketPeriodEnum? Period { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixPersonDataDTO Person { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> PersonAddressRegistrations { get; set; }

        [RequiredIf(nameof(Type), "msgRequired", typeof(ErrorResources), TicketTypeEnum.UNDER14)]
        public RegixPersonDataDTO RepresentativePerson { get; set; }

        [RequiredIf(nameof(Type), "msgRequired", typeof(ErrorResources), TicketTypeEnum.UNDER14)]
        public List<AddressRegistrationDTO> RepresentativePersonAddressRegistrations { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? ValidFrom { get; set; }

        public int? AssociationId { get; set; }

        public string TerritoryUnit { get; set; } // not sent from UI

        public string Address { get; set; } // not sent from UI

        public string Code { get; set; } // not sent from UI
    }
}
