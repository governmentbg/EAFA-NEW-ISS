using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.Duplicates
{
    public class DuplicatesApplicationDTO : DuplicatesApplicationBaseRegixDataDTO, IDeliverableApplication
    {
        public bool IsOnlineApplication { get; set; }

        public bool IsPaid { get; set; }

        public bool HasDelivery { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Reason { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public override PageCodeEnum? PageCode { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.DupFirstSaleBuyer, PageCodeEnum.DupFirstSaleCenter)]
        public BuyerDuplicateDataDTO Buyer { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.DupCommFish, PageCodeEnum.DupRightToFishThirdCountry, PageCodeEnum.DupPoundnetCommFish)]
        public PermitDuplicateDataDTO Permit { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.DupRightToFishResource, PageCodeEnum.DupPoundnetCommFishLic, PageCodeEnum.DupCatchQuataSpecies)]
        public PermitLicenseDuplicateDataDTO PermitLicence { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.CompetencyDup)]
        public QualifiedFisherDuplicateDataDTO QualifiedFisher { get; set; }

        public ApplicationPaymentInformationDTO PaymentInformation { get; set; }

        public ApplicationBaseDeliveryDTO DeliveryData { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
