using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees
{
    public class PenalDecreeEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? AuanId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? TypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? IssuerUserId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string DecreeNum { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? IssueDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? EffectiveDate { get; set; }

        public List<int> SanctionTypeIds { get; set; }

        public decimal? FineAmount { get; set; }

        public List<PenalDecreeSeizedFishDTO> SeizedFish { get; set; }

        public List<PenalDecreeSeizedFishDTO> SeizedAppliance { get; set; }

        public List<PenalDecreeSeizedFishingGearDTO> SeizedFishingGear { get; set; }

        public List<PenalDecreeFishCompensationDTO> FishCompensations { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AuanViolatedRegulationDTO> AuanViolatedRegulations { get; set; }

        public List<AuanViolatedRegulationDTO> DecreeViolatedRegulations { get; set; }

        public List<AuanViolatedRegulationDTO> FishCompensationViolatedRegulations { get; set; }

        public decimal? CompensationAmount { get; set; }

        public bool? IsRecurrentViolation { get; set; }

        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string IssuerPosition { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string SanctionDescription { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ConstatationComments { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string MinorCircumstancesDescription { get; set; }

        public PenalDecreeDeliveryDataDTO DeliveryData { get; set; }

        public List<PenalDecreeStatusEditDTO> Statuses { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
