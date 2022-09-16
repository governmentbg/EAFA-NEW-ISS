using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CatchQuotas
{
    public class YearlyQuotaEditDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FishId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? Year { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? QuotaValueKg { get; set; }

        public decimal? LeftoverValueKg { get; set; }

        [StringLength(1000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ChangeBasis { get; set; }

        public List<NomenclatureDTO> UnloadPorts { get; set; }

        public IEnumerable<FileInfoDTO> Files { get; set; }
    }
}
