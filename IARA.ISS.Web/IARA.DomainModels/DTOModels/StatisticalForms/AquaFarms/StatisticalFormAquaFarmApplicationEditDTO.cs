using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms
{
    public class StatisticalFormAquaFarmApplicationEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ApplicationId { get; set; }

        public bool IsOnlineApplication { get; set; }

        public ApplicationSubmittedByDTO SubmittedBy { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string SubmittedByWorkPosition { get; set; }

        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? AquacultureFacilityId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? Year { get; set; }

        public decimal? BreedingMaterialDeathRate { get; set; }

        public decimal? ConsumationFishDeathRate { get; set; }

        public decimal? BroodstockDeathRate { get; set; }

        public List<StatisticalFormGivenMedicineDTO> Medicine { get; set; }

        public List<StatisticalFormAquaFarmFishOrganismDTO> ProducedFishOrganism { get; set; }

        public List<StatisticalFormAquaFarmFishOrganismDTO> SoldFishOrganism { get; set; }

        public List<StatisticalFormAquaFarmFishOrganismDTO> UnrealizedFishOrganism { get; set; }

        public List<StatisticalFormAquaFarmBroodstockDTO> Broodstock { get; set; }

        public List<StatisticalFormAquaFarmInstallationSystemFullDTO> InstallationSystemFull { get; set; }

        public List<StatisticalFormAquaFarmInstallationSystemNotFullDTO> InstallationSystemNotFull { get; set; }

        public string MedicineComments { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<StatisticalFormNumStatGroupDTO> NumStatGroups { get; set; }

        public List<StatisticalFormEmployeeInfoGroupDTO> EmployeeInfoGroups { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
