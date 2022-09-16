using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.AquacultureFacilities.Installations
{
    public class AquacultureInstallationNetCageDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? NetCageTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public AquacultureInstallationNetCageShapesEnum? Shape { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? Count { get; set; }

        [RequiredIf(nameof(Shape), "msgRequired", typeof(ErrorResources), AquacultureInstallationNetCageShapesEnum.Circular)]
        public decimal? Radius { get; set; }

        [RequiredIf(nameof(Shape), "msgRequired", typeof(ErrorResources), AquacultureInstallationNetCageShapesEnum.Rectangular)]
        public decimal? Length { get; set; }

        [RequiredIf(nameof(Shape), "msgRequired", typeof(ErrorResources), AquacultureInstallationNetCageShapesEnum.Rectangular)]
        public decimal? Width { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? Height { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? Area { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? Volume { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
