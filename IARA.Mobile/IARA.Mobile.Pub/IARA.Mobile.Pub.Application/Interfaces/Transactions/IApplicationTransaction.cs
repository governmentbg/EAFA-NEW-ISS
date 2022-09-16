using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IApplicationTransaction
    {
        Task<ApplicationStatusChangeDto> InitiateApplicationCorrections(int applicationId);
    }
}
