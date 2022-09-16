using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CatchQuotas
{
    public class ShipQuotaEditDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? QuotaId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? ShipQuotaSize { get; set; }

        public decimal? LeftoverQuotaSize { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ChangeBasis { get; set; }

        public decimal? UnloadedByCurrentDateKg { get; set; }

        public decimal? UnloadedByCurrentDatePercent { get; set; }
    }
}
