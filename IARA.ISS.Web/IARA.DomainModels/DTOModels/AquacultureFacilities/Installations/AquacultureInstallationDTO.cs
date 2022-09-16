using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.AquacultureFacilities.Installations
{
    public class AquacultureInstallationDTO
    {
        public int? Id { get; set; }

        public string InstallationTypeName { get; set; }

        public decimal? TotalArea { get; set; }

        public decimal? TotalVolume { get; set; }

        public int? TotalCount { get; set; }

        public bool HasValidationErrors { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
