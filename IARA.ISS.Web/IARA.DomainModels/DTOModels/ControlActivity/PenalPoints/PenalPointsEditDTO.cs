using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalPoints
{
    public class PenalPointsEditDTO
    {
        public int? Id { get; set; }

        public int? AuanId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? DecreeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public PointsTypeEnum? PointsType { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string DecreeNum { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsIncreasePoints { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? IssueDate { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int? TerritoryUnitId { get; set; }

        public int? ShipId { get; set; }

        public int? PermitId { get; set; }

        public bool? IsPermitOwner { get; set; }

        public int? PermitOwnerPersonId { get; set; }

        public int? PermitOwnerLegalId { get; set; }

        public int? QualifiedFisherId { get; set; }

        public int? PermitLicenseId { get; set; }

        public RegixPersonDataDTO PersonOwner { get; set; }

        public RegixLegalDataDTO LegalOwner { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? PointsAmount { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Issuer { get; set; }

        public List<PenalPointsAppealDTO> AppealStatuses { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
