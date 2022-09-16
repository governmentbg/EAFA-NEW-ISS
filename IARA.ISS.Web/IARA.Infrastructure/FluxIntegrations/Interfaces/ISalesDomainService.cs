using System.Threading.Tasks;
using IARA.Flux.Models;

namespace IARA.Infrastructure.FluxIntegrations.Interfaces
{
    public interface ISalesDomainService
    {
        Task<bool> ReportSalesDocument(FLUXSalesReportMessageType message);

        FLUXSalesReportMessageType FindSalesDocuments(FLUXSalesQueryMessageType query);

        bool MustSendSalesReport(int? shipId = null, decimal? length = null);
    }
}
