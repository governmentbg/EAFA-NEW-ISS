using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms
{
    public class StatisticalFormAquaFarmBroodstockDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? InstallationTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FishTypeId { get; set; }

        public int? MaleCount { get; set; }

        public decimal? MaleWeight { get; set; }

        public int? MaleAge { get; set; }

        public int? FemaleCount { get; set; }

        public decimal? FemaleWeight { get; set; }

        public int? FemaleAge { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
