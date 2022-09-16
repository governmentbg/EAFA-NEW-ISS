using IARA.DomainModels.DTOModels.FishingCapacity;

namespace IARA.Infrastructure.Services
{
    internal class CapacityHolderRegixHelper : FishingCapacityHolderRegixDataDTO
    {
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
    }
}
