using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanDeliveryDataDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public InspDeliveryTypesEnum? DeliveryType { get; set; }

        [RequiredIf(nameof(DeliveryType), "msgRequired", typeof(ErrorResources), InspDeliveryTypesEnum.ByMail)]
        public DateTime? SentDate { get; set; }

        [RequiredIf(nameof(DeliveryType), "msgRequired", typeof(ErrorResources), InspDeliveryTypesEnum.StateService)]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ReferenceNum { get; set; }

        [RequiredIf(nameof(DeliveryType), "msgRequired", typeof(ErrorResources), InspDeliveryTypesEnum.StateService)]
        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string StateService { get; set; }

        [RequiredIf(nameof(DeliveryType), "msgRequired", typeof(ErrorResources), InspDeliveryTypesEnum.Office)]
        public int? TerritoryUnitId { get; set; }

        [RequiredIf(nameof(DeliveryType), "msgRequired", typeof(ErrorResources), InspDeliveryTypesEnum.ByMail)]
        public AddressRegistrationDTO Address { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsDelivered { get; set; }

        [RequiredIf(nameof(DeliveryType), "msgRequired", typeof(ErrorResources), InspDeliveryTypesEnum.Personal, InspDeliveryTypeGroupsEnum.AUAN)]
        public DateTime? DeliveryDate { get; set; }

        public InspDeliveryConfirmationTypesEnum? ConfirmationType { get; set; }

        public bool? IsEDeliveryRequested { get; set; }

        public DateTime? RefusalDate { get; set; }

        [RequiredIf(nameof(DeliveryType), "msgRequired", typeof(ErrorResources), InspDeliveryTypesEnum.Refusal, InspDeliveryTypeGroupsEnum.AUAN)]
        public List<AuanWitnessDTO> RefusalWitnesses { get; set; }
    }
}
