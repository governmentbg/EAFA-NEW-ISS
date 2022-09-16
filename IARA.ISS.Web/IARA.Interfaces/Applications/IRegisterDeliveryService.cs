using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.Interfaces.Applications
{
    public interface IRegisterDeliveryService
    {
        Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode);
    }
}
