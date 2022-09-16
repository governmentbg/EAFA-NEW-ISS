using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class CommercialFishingApplicationEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ApplicationId { get; set; }

        public int? PermitLicensePermitId { get; set; }

        public string PermitLicensePermitNumber { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public PageCodeEnum? PageCode { get; set; }

        public bool IsPaid { get; set; }

        public bool HasDelivery { get; set; }

        public bool IsOnlineApplication { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        public int? QualifiedFisherId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? QualifiedFisherSameAsSubmittedFor { get; set; }

        public EgnLncDTO QualifiedFisherIdentifier { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string QualifiedFisherFirstName { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string QualifiedFisherMiddleName { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string QualifiedFisherLastName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipId { get; set; }

        [RequiredIf(
            nameof(PageCode),
            "msgRequired",
            typeof(ErrorResources), PageCodeEnum.PoundnetCommFish, PageCodeEnum.PoundnetCommFishLic)]
        public int? PoundNetId { get; set; }

        [RequiredIf(
            nameof(PageCode),
            "msgRequired",
            typeof(ErrorResources), PageCodeEnum.RightToFishResource, PageCodeEnum.PoundnetCommFish, PageCodeEnum.PoundnetCommFishLic, PageCodeEnum.CatchQuataSpecies)]
        public bool? IsHolderShipOwner { get; set; }

        [RequiredIf(nameof(IsHolderShipOwner), "msgRequired", typeof(ErrorResources), false)]
        public HolderGroundForUseDTO ShipGroundForUse { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.PoundnetCommFish)]
        public HolderGroundForUseDTO PoundNetGroundForUse { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.CatchQuataSpecies)]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string UnloaderPhoneNumber { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? WaterTypeId { get; set; }

        [RequiredIf(nameof(PageCode),
                    "msgRequired",
                    typeof(ErrorResources),
                    PageCodeEnum.RightToFishResource,
                    PageCodeEnum.PoundnetCommFishLic)]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<int> AquaticOrganismTypeIds { get; set; }

        [RequiredIf(nameof(PageCode),
                    "msgRequired",
                    typeof(ErrorResources),
                    PageCodeEnum.CatchQuataSpecies)]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<QuotaAquaticOrganismDTO> QuotaAquaticOrganisms { get; set; }

        [RequiredIf(nameof(PageCode),
            "msgRequired",
            typeof(ErrorResources),
            PageCodeEnum.RightToFishResource,
            PageCodeEnum.PoundnetCommFishLic,
            PageCodeEnum.CatchQuataSpecies)]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<FishingGearDTO> FishingGears { get; set; }

        public ApplicationPaymentInformationDTO PaymentInformation { get; set; }

        public ApplicationBaseDeliveryDTO DeliveryData { get; set; }

        public List<FileInfoDTO> Files { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string StatusReason { get; set; }
    }
}
