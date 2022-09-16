using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class ShipLogBookPageEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string PageNumber { get; set; }

        public int PermitLicenseId { get; set; }

        /// <summary>
        /// For UI in dialog
        /// </summary>
        public string PermitLicenseNumber { get; set; }

        public LogBookPageStatusesEnum StatusCode { get; set; }

        /// <summary>
        /// Only for UI
        /// </summary>
        public string Status { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? LogBookPermitLicenseId { get; set; }

        /// <summary>
        /// For UI in dialog
        /// </summary>
        public string PermitLicenseName { get; set; }

        public WaterTypesEnum PermitLicenseWaterType { get; set; }

        public string PermitLicenseWaterTypeName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? LogBookId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? FillDate { get; set; }

        public DateTime? IaraAcceptanceDateTime { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipId { get; set; }

        public string ShipName { get; set; } // For UI in dialog

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FishingGearRegisterId { get; set; }

        public int? FishingGearCount { get; set; }

        public int? FishingGearHookCount { get; set; }

        public int? PartnerShipId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? FishTripStartDateTime { get; set; }

        public DateTime? FishTripEndDateTime { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? DeparturePortId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ArrivalPortId { get; set; }

        [RequiredIf(nameof(AllCatchIsTransboarded), "msgRequired", typeof(ErrorResources), false)]
        public DateTime? UnloadDateTime { get; set; }

        [RequiredIf(nameof(AllCatchIsTransboarded), "msgRequired", typeof(ErrorResources), false)]
        public int? UnloadPortId { get; set; }

        public bool AllCatchIsTransboarded { get; set; }

        public List<CatchRecordDTO> CatchRecords { get; set; }

        public int? OriginDeclarationId { get; set; }

        public List<OriginDeclarationFishDTO> OriginDeclarationFishes { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
