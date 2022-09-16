using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.Buyers
{
    public class BuyerEditDTO
    {
        public int? Id { get; set; }

        public int ApplicationId { get; set; }

        public bool IsOnlineApplication { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? TerritoryUnitId { get; set; }

        public PageCodeEnum PageCode { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public BuyerTypesEnum? BuyerType { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public BuyerStatusesEnum? BuyerStatus { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.RegFirstSaleBuyer)]
        public bool? HasUtility { get; set; }

        [RequiredIf(nameof(HasUtility), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string UtilityName { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.RegFirstSaleBuyer)]
        public bool? HasVehicle { get; set; }

        [RequiredIf(nameof(HasVehicle), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string VehicleNumber { get; set; }

        public string UrorrNumber { get; set; }

        public string RegistrationNumber { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? RegistrationDate { get; set; }

        public bool IsActive { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }

        public List<UsageDocumentDTO> PremiseUsageDocuments { get; set; }

        public int? PremiseAddressId { get; set; }

        [RequiredIf(nameof(HasUtility), "msgRequired", typeof(ErrorResources), true)]
        public AddressRegistrationDTO PremiseAddress { get; set; }

        public int? SubmittedForLegalId { get; set; }

        public int? SubmittedForPersonId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.RegFirstSaleCenter)]
        public RegixPersonDataDTO Organizer { get; set; }

        public int? OrganizerPersonId { get; set; }

        public bool? OrganizerSameAsSubmittedBy { get; set; }

        public int? AgentId { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.RegFirstSaleBuyer)]
        public RegixPersonDataDTO Agent { get; set; }

        public bool? IsAgentSameAsSubmittedBy { get; set; }

        public bool? IsAgentSameAsSubmittedForCustodianOfProperty { get; set; }

        public decimal? AnnualTurnover { get; set; }

        [RequiredIf(nameof(HasUtility), "msgRequired", typeof(ErrorResources), true)]
        public List<CommonDocumentDTO> BabhLawLicenseDocuments { get; set; }

        [RequiredIf(nameof(HasVehicle), "msgRequired", typeof(ErrorResources), true)]
        public List<CommonDocumentDTO> VeteniraryVehicleRegLicenseDocuments { get; set; }

        public List<CancellationHistoryEntryDTO> CancellationHistory { get; set; }

        public List<LogBookEditDTO> LogBooks { get; set; }

        public List<DuplicatesEntryDTO> DuplicateEntries { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
