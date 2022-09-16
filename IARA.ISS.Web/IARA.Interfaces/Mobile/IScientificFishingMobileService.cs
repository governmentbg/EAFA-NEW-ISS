using IARA.DomainModels.DTOModels.ScientificFishing;

namespace IARA.Interfaces
{
    public interface IScientificFishingMobileService
    {
        ScientificFishingMobileDataDTO GetAllPermits(int currentUserId);

        int AddOuting(ScientificFishingOutingDTO outing, int currentUserId);

        void EditOuting(ScientificFishingOutingDTO outing, int currentUserId);

        void DeleteOuting(int id, int currentUserId);
    }
}
