using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class ApplicationTransaction : BaseTransaction, IApplicationTransaction
    {
        public ApplicationTransaction(BaseTransactionProvider provider) : base(provider)
        {
        }

        public async Task<ApplicationStatusChangeDto> InitiateApplicationCorrections(int applicationId)
        {
            return await RestClient.PatchAsync<ApplicationStatusChangeDto>($"Application/InitiateApplicationCorrections", new { applicationId })
               .GetHttpContent();
        }
    }
}
