using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterEditDTO
    {
        public int? Id { get; set; }

        [RequiredIf(nameof(IsThirdPartyShip), "msgRequired", typeof(ErrorResources), false)]
        public int? ShipUID { get; set; }

        [RequiredIf(nameof(IsThirdPartyShip), "msgRequired", typeof(ErrorResources), false)]
        public int? ApplicationId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsThirdPartyShip { get; set; }

        public ShipEventTypeEnum? EventType { get; set; }

        public DateTime? EventDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string CFR { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ExternalMark { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(14, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string RegistrationNumber { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? RegistrationDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FleetTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FleetSegmentId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CountryFlagId { get; set; }

        [StringLength(7, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string IRCSCallSign { get; set; }

        [StringLength(9, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string MMSI { get; set; }

        [StringLength(7, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string UVI { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasAIS { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasERS { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasVMS { get; set; }

        public int? VesselTypeId { get; set; }

        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string RegLicenceNum { get; set; }

        public DateTime? RegLicenceDate { get; set; }

        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string RegLicencePublisher { get; set; }

        [StringLength(10, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string RegLicencePublishVolume { get; set; }

        [StringLength(10, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string RegLicencePublishPage { get; set; }

        [StringLength(10, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string RegLicencePublishNum { get; set; }

        public DateTime? ExploitationStartDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public short? BuildYear { get; set; }

        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string BuildPlace { get; set; }

        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string AdminDecisionNum { get; set; }

        public DateTime? AdminDecisionDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? PublicAidTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? PortId { get; set; }

        public int? StayPortId { get; set; }

        public int? SailAreaId { get; set; }

        public string SailAreaName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? TotalLength { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? TotalWidth { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? GrossTonnage { get; set; }

        public decimal? NetTonnage { get; set; }

        public decimal? OtherTonnage { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? BoardHeight { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? ShipDraught { get; set; }

        public decimal? LengthBetweenPerpendiculars { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? MainEnginePower { get; set; }

        public decimal? AuxiliaryEnginePower { get; set; }

        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string MainEngineNum { get; set; }

        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string MainEngineModel { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? MainFishingGearId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? AdditionalFishingGearId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? HullMaterialId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FuelTypeId { get; set; }

        public int? TotalPassengerCapacity { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CrewCount { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasControlCard { get; set; }

        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ControlCardNum { get; set; }

        [RequiredIf(nameof(HasControlCard), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? ControlCardDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasValidityCertificate { get; set; }

        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [RequiredIf(nameof(HasValidityCertificate), "msgRequired", typeof(ErrorResources), true)]
        public string ControlCardValidityCertificateNum { get; set; }

        [RequiredIf(nameof(HasValidityCertificate), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? ControlCardValidityCertificateDate { get; set; }

        [RequiredIf(nameof(HasValidityCertificate), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? ControlCardDateOfLastAttestation { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasFoodLawLicence { get; set; }

        [RequiredIf(nameof(HasFoodLawLicence), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string FoodLawLicenceNum { get; set; }

        [RequiredIf(nameof(HasFoodLawLicence), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? FoodLawLicenceDate { get; set; }

        public int? ShipAssociationId { get; set; }

        [RequiredIf(nameof(EventType), "msgRequired", typeof(ErrorResources), ShipEventTypeEnum.IMP)]
        public int? ImportCountryId { get; set; }

        [RequiredIf(nameof(EventType), "msgRequired", typeof(ErrorResources), ShipEventTypeEnum.EXP)]
        public int? ExportCountryId { get; set; }

        [RequiredIf(nameof(EventType), "msgRequired", typeof(ErrorResources), ShipEventTypeEnum.EXP)]
        public ShipExportTypeEnum? ExportType { get; set; }

        public AcquiredFishingCapacityDTO AcquiredFishingCapacity { get; set; }

        public FishingCapacityFreedActionsDTO RemainingCapacityAction { get; set; }

        [RequiredIf(nameof(EventType), "msgRequired", typeof(ErrorResources), ShipEventTypeEnum.DES, ShipEventTypeEnum.RET, ShipEventTypeEnum.EXP)]
        public bool? IsNoApplicationDeregistration { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }

        [RequiredIf(nameof(EventType), "msgRequired", typeof(ErrorResources), ShipEventTypeEnum.DES, ShipEventTypeEnum.RET)]
        public CancellationDetailsDTO CancellationDetails { get; set; }

        public bool HasFishingPermit { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsForbiddenForRSR { get; set; }

        [RequiredIf(nameof(IsForbiddenForRSR), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? ForbiddenStartDate { get; set; }

        [RequiredIf(nameof(IsForbiddenForRSR), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? ForbiddenEndDate { get; set; }

        [RequiredIf(nameof(IsForbiddenForRSR), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ForbiddenReason { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<ShipOwnerDTO> Owners { get; set; }

        public List<ShipRegisterUserDTO> ShipUsers { get; set; }

        public List<ShipRegisterUsedCertificateDTO> UsedCapacityCertificates { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
