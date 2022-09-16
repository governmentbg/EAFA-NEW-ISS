using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms
{
    public class StatisticalFormAquaFarmInstallationSystemNotFullDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? InstallationTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsInstallationUsedBreedingMaterial { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsInstallationUsedConsumationFish { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
