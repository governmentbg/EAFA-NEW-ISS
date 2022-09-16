using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanRegisterEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? InspectionId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string AuanNum { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? DraftDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(400, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string LocationDescription { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public AuanInspectedEntityDTO InspectedEntity { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AuanWitnessDTO> AuanWitnesses { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AuanViolatedRegulationDTO> ViolatedRegulations { get; set; }

        public List<AuanConfiscatedFishDTO> ConfiscatedFish { get; set; }

        public List<AuanConfiscatedFishDTO> ConfiscatedAppliance { get; set; }

        public List<AuanConfiscatedFishingGearDTO> ConfiscatedFishingGear { get; set; }


        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ConstatationComments { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string OffenderComments { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public AuanDeliveryDataDTO DeliveryData { get; set; }

        public bool? HasObjection { get; set; }

        [RequiredIf(nameof(HasObjection), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? ObjectionDate { get; set; }

        [RequiredIf(nameof(HasObjection), "msgRequired", typeof(ErrorResources), true)]
        public AuanObjectionResolutionTypesEnum? ResolutionType { get; set; }

        [RequiredIf(nameof(HasObjection), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ResolutionNum { get; set; }

        [RequiredIf(nameof(HasObjection), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? ResolutionDate { get; set; }

        public int? StatusId { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
