using System.Threading.Tasks;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IApplicationTransaction
    {
        Task<ApplicationStatusChangeDto> InitiateApplicationCorrections(int applicationId);
    }
}
