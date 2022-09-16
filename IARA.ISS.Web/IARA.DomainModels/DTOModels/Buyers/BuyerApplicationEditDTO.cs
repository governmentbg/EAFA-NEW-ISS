using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.Buyers
{
    public class BuyerApplicationEditDTO
    {
        public int? Id { get; set; }

        public int? ApplicationId { get; set; }

        public bool IsOnlineApplication { get; set; }

        public PageCodeEnum PageCode { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByDTO SubmittedBy { get; set; }

        public int? SubmittedForLegalId { get; set; }

        public int? SubmittedForPersonId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        public int? OrganizerPersonId { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.RegFirstSaleCenter)]
        public RegixPersonDataDTO Organizer { get; set; }

        public bool? OrganizerSameAsSubmittedBy { get; set; }

        public int? AgentId { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.RegFirstSaleBuyer)]
        public RegixPersonDataDTO Agent { get; set; }

        public bool? IsAgentSameAsSubmittedBy { get; set; }

        public bool? IsAgentSameAsSubmittedForCustodianOfProperty { get; set; }

        public UsageDocumentDTO PremiseUsageDocument { get; set; }

        public int? PremiseAddressId { get; set; }

        [RequiredIf(nameof(HasUtility), "msgRequired", typeof(ErrorResources), true)]
        public AddressRegistrationDTO PremiseAddress { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.RegFirstSaleBuyer, PageCodeEnum.RegFirstSaleCenter)]
        public bool? HasUtility { get; set; }

        [RequiredIf(nameof(HasUtility), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string UtilityName { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.RegFirstSaleBuyer, PageCodeEnum.RegFirstSaleCenter)]
        public bool? HasVehicle { get; set; }

        [RequiredIf(nameof(HasVehicle), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string VehicleNumber { get; set; }

        public ApplicationPaymentInformationDTO PaymentInformation { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationBaseDeliveryDTO DeliveryData { get; set; }

        [RequiredIf(nameof(HasUtility), "msgRequired", typeof(ErrorResources), true)]
        public CommonDocumentDTO BabhLawLicenseDocumnet { get; set; }

        [RequiredIf(nameof(HasVehicle), "msgRequired", typeof(ErrorResources), true)]
        public CommonDocumentDTO VeteniraryVehicleRegLicenseDocument { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }

        public List<FileInfoDTO> Files { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string StatusReason { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasDelivery { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsPaid { get; set; }

        public decimal? AnnualTurnover { get; set; }
    }
}
