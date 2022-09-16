using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.Flux.Models;

namespace IARA.Infrastructure.FluxIntegrations.Interfaces
{
    public interface IFishingActivitiesDomainService
    {
        FLUXFAReportMessageType ProcessFluxFAQuery(FLUXFAQueryMessageType query);
        void ReceiveFishingActivitiesReport(FLUXFAReportMessageType report);
        Task<bool> ReportFishingActivities(ShipLogBookPageFLUXFieldsDTO logBookPageData);
    }
}
