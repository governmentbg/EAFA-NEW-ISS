using IARA.DomainModels.DTOModels.FishingCapacity;

namespace IARA.Infrastructure.Services
{
    internal class CapacityHolderHelper : FishingCapacityHolderDTO
    {
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
    }
}
