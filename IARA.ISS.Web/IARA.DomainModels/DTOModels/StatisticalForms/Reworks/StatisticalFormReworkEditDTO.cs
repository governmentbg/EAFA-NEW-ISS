using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.StatisticalForms.Reworks
{
    public class StatisticalFormReworkEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ApplicationId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        public bool IsOnlineApplication { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? Year { get; set; }

        public string FormNum { get; set; }

        public string VetRegistrationNum { get; set; }

        public string LicenceNum { get; set; }

        public DateTime? LicenceDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? TotalRawMaterialTons { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? TotalReworkedProductTons { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? TotalYearTurnover { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<StatisticalFormReworkRawMaterialDTO> RawMaterial { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<StatisticalFormReworkProductDTO> Products { get; set; }
        public List<StatisticalFormEmployeeInfoGroupDTO> EmployeeInfoGroups { get; set; }
        public List<StatisticalFormNumStatGroupDTO> NumStatGroups { get; set; }

        public List<StatisticalFormEmployeeInfoDTO> WorkDayDuration { get; set; }

        public List<StatisticalFormEmployeeInfoDTO> EmployeeAge { get; set; }

        public List<StatisticalFormEmployeeInfoDTO> EmployeeEducation { get; set; }

        public List<StatisticalFormEmployeeInfoDTO> EmployeeNationality { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
