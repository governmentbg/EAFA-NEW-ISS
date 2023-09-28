using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Infrastructure.FluxIntegrations.Validations.Common;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations
{
    public class FluxSalesQueryValidations : FluxValidations<FLUXSalesQueryMessageType>
    {
        public FluxSalesQueryValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider)
            : base(db, serviceProvider)
        { }

        public static void UpdateFluxResponse(FLUXSalesResponseMessageType response, List<FluxValidationResult> validations)
        {
            UpdateFluxResponse(response.FLUXResponseDocument, validations);
        }

        [FluxValidation("SALE-L00-00-9998",
                        "L03",
                        FluxValidationSeverity.Error,
                        "FLUXSalesQueryMessage",
                        "No data corresponding to the query.")]
        public bool SALE00009998(FLUXSalesQueryMessageType query)
        {
            ISalesDomainService service = ServiceProvider.GetService<ISalesDomainService>();
            return service.QueryHasResults(query);
        }

        [FluxValidation("SALE-L03-00-0400",
                        "L03",
                        FluxValidationSeverity.Error,
                        "Sales Query/Identification",
                        "The identification must be unique and not already exist",
                        true)]
        public bool SALE03000400(FLUXSalesQueryMessageType query)
        {
            if (query.SalesQuery != null && query.SalesQuery.ID != null && !string.IsNullOrEmpty(query.SalesQuery.ID.Value))
            {
                bool exists = (from fvms in Db.Fluxfvmsrequests
                               where fvms.RequestUuid.ToString() == query.SalesQuery.ID.Value
                               select 1).Any();

                return !exists;
            }

            return true;
        }
    }
}
