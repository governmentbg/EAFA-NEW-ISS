using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.AquacultureFacilities.Installations
{
    public class AquacultureInstallationEditDTO : AquacultureInstallationDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public AquacultureInstallationTypeEnum? InstallationType { get; set; }

        [RequiredIf(nameof(InstallationType), "msgRequired", typeof(ErrorResources), AquacultureInstallationTypeEnum.Basins)]
        public List<AquacultureInstallationBasinDTO> Basins { get; set; }

        [RequiredIf(nameof(InstallationType), "msgRequired", typeof(ErrorResources), AquacultureInstallationTypeEnum.NetCages)]
        public List<AquacultureInstallationNetCageDTO> NetCages { get; set; }

        [RequiredIf(nameof(InstallationType), "msgRequired", typeof(ErrorResources), AquacultureInstallationTypeEnum.Aquariums)]
        public AquacultureInstallationAquariumDTO Aquariums { get; set; }

        [RequiredIf(nameof(InstallationType), "msgRequired", typeof(ErrorResources), AquacultureInstallationTypeEnum.Collectors)]
        public List<AquacultureInstallationCollectorDTO> Collectors { get; set; }

        [RequiredIf(nameof(InstallationType), "msgRequired", typeof(ErrorResources), AquacultureInstallationTypeEnum.Rafts)]
        public List<AquacultureInstallationRaftDTO> Rafts { get; set; }

        [RequiredIf(nameof(InstallationType), "msgRequired", typeof(ErrorResources), AquacultureInstallationTypeEnum.Dams)]
        public AquacultureInstallationDamDTO Dams { get; set; }

        [RequiredIf(nameof(InstallationType), "msgRequired", typeof(ErrorResources), AquacultureInstallationTypeEnum.RecirculatorySystems)]
        public List<AquacultureInstallationRecirculatorySystemDTO> RecirculatorySystems { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }
    }
}
