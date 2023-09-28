using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Validations.Common;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations
{
    public class FluxSalesReportValidations : FluxValidations<FLUXSalesReportMessageType>
    {
        public FluxSalesReportValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider) 
            : base(db, serviceProvider)
        {}

        public static void UpdateFluxResponse(FLUXSalesResponseMessageType response, List<FluxValidationResult> validations)
        {
            UpdateFluxResponse(response.FLUXResponseDocument, validations);
        }

        [FluxValidation("SALE-L03-00-0010",
                        "L03",
                        FluxValidationSeverity.Error,
                        "FLUX_ReportDocument/Identification",
                        "The identification must be unique and not already exist",
                        true)]
        public bool SALE03000010(FLUXSalesReportMessageType report)
        {
            if (report.FLUXReportDocument != null
                && report.FLUXReportDocument.ID != null
                && report.FLUXReportDocument.ID.Length != 0
                && !string.IsNullOrEmpty(report.FLUXReportDocument.ID[0].Value))
            {
                bool exists = (from fvms in Db.Fluxfvmsrequests
                               where fvms.RequestUuid.ToString() == report.FLUXReportDocument.ID[0].Value
                               select 1).Any();

                return !exists;
            }

            return true;
        }
    }
}
