using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.StatisticalForms.FishVessels
{
    public class StatisticalFormFishVesselApplicationEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ApplicationId { get; set; }

        public bool IsOnlineApplication { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByDTO SubmittedBy { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string SubmittedByWorkPosition { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipId { get; set; }

        public int? Year { get; set; }

        public int? RegistrationNum { get; set; }

        public int? ShipYears { get; set; }

        public decimal? ShipPrice { get; set; }

        public int? ShipLengthId { get; set; }

        public int? ShipTonnageId { get; set; }

        public bool? HasEngine { get; set; }

        public int? FuelTypeId { get; set; }

        public decimal? ShipEnginePower { get; set; }

        public decimal? FreeLaborAmount { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsShipHolderPartOfCrew { get; set; }

        public string ShipHolderPosition { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsFishingMainActivity { get; set; }

        public int? WorkedOutHours { get; set; }

        public List<StatisticalFormsSeaDaysDTO> SeaDays { get; set; }

        public List<StatisticalFormEmployeeInfoGroupDTO> EmployeeInfoGroups { get; set; }

        public List<StatisticalFormNumStatGroupDTO> NumStatGroups { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
