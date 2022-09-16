using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms
{
    public class StatisticalFormAquaFarmFishOrganismDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? InstallationTypeId { get; set; }

        public AquaFarmFishOrganismReportTypeEnum? ReportType { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FishTypeId { get; set; }

        public int? FishLarvaeCount { get; set; }

        public int? OneStripBreedingMaterialCount { get; set; }

        public decimal? OneStripBreedingMaterialWeight { get; set; }

        public int? OneYearBreedingMaterialCount { get; set; }

        public decimal? OneYearBreedingMaterialWeight { get; set; }

        public decimal? ForConsumption { get; set; }

        public decimal? CaviarForConsumption { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
