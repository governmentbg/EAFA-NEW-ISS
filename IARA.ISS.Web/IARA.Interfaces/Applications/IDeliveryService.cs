using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.Interfaces
{
    public interface IDeliveryService : IService
    {
        bool HasApplicationDelivery(int applicationId);

        ApplicationDeliveryDTO GetApplicationDeliveryData(int applicationId);

        ApplicationDeliveryDTO GetDeliveryData(int deliveryId);

        Task<int> UpdateDeliveryData(ApplicationDeliveryDTO deliveryData, int deliveryId, bool sendEDelivery = false);

        Task<bool> HasSubmittedForEDelivery(int deliveryTypeId, ApplicationSubmittedForRegixDataDTO submittedFor, ApplicationSubmittedByRegixDataDTO submittedBy);

        Task<bool> HasSubmittedForEDelivery(int deliveryId, int deliveryTypeId);

        Task<bool> HasPersonAccessToEDeliveryAsync(string identifier);

        Task<bool> HasLegalAccessToEDeliveryAsync(string identifier);
    }
}
