using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class CommercialFishingEditDTO
    {
        public int? Id { get; set; }

        public int ApplicationId { get; set; }

        public bool IsOnlineApplication { get; set; }

        public PageCodeEnum PageCode { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public CommercialFishingTypesEnum? Type { get; set; }

        [RequiredIf(
            nameof(PageCode),
            "msgRequired",
            typeof(ErrorResources), PageCodeEnum.RightToFishResource, PageCodeEnum.PoundnetCommFishLic, PageCodeEnum.CatchQuataSpecies)]
        public int? PermitLicensePermitId { get; set; }

        public string PermitRegistrationNumber { get; set; }

        public string PermitLicenseRegistrationNumber { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? IssueDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? ValidFrom { get; set; }

        [RequiredIf(nameof(IsPermitUnlimited), "msgRequired", typeof(ErrorResources), false)]
        public DateTime? ValidTo { get; set; }

        public List<SuspensionDataDTO> Suspensions { get; set; }

        [RequiredIf(
            nameof(PageCode),
            "msgRequired",
            typeof(ErrorResources), PageCodeEnum.CommFish, PageCodeEnum.PoundnetCommFish, PageCodeEnum.RightToFishThirdCountry)]
        public bool? IsPermitUnlimited { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        [RequiredIf(
            nameof(PageCode),
            "msgRequired",
            typeof(ErrorResources), PageCodeEnum.RightToFishResource, PageCodeEnum.PoundnetCommFish, PageCodeEnum.PoundnetCommFishLic, PageCodeEnum.CatchQuataSpecies)]
        public bool? IsHolderShipOwner { get; set; }

        public HolderGroundForUseDTO ShipGroundForUse { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.PoundnetCommFish)]
        public HolderGroundForUseDTO PoundNetGroundForUse { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? QualifiedFisherId { get; set; }

        public EgnLncDTO QualifiedFisherIdentifier { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string QualifiedFisherFirstName { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string QualifiedFisherMiddleName { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string QualifiedFisherLastName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? QualifiedFisherSameAsSubmittedFor { get; set; }

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

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.PoundnetCommFish, PageCodeEnum.PoundnetCommFishLic)]
        public int? PoundNetId { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.CatchQuataSpecies)]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string UnloaderPhoneNumber { get; set; }

        [RequiredIf(nameof(PageCode),
            "msgRequired",
            typeof(ErrorResources),
            PageCodeEnum.RightToFishResource,
            PageCodeEnum.PoundnetCommFishLic,
            PageCodeEnum.CatchQuataSpecies)]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<FishingGearDTO> FishingGears { get; set; }

        public List<CommercialFishingLogBookEditDTO> LogBooks { get; set; }

        public List<DuplicatesEntryDTO> DuplicateEntries { get; set; }

        [XmlIgnore]
        public List<FileInfoDTO> Files { get; set; }
    }
}
