using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Infrastructure.FluxIntegrations.Validations.Common;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations
{
    public class FluxFAQueryValidations : FluxValidations<FLUXFAQueryMessageType>
    {
        public FluxFAQueryValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider)
            : base(db, serviceProvider)
        { }

        [FluxValidation("FA-L03-00-9998",
                        "L03",
                        FluxValidationSeverity.Error,
                        "FLUXFAQueryMessage",
                        "No results for query")]
        public bool FAL03009998(FLUXFAQueryMessageType query)
        {
            IFishingActivitiesDomainService service = ServiceProvider.GetService<IFishingActivitiesDomainService>();
            return service.FAQueryHasResults(query);
        }

        [FluxValidation("FA-L03-00-0650",
                        "L03",  
                        FluxValidationSeverity.Error,
                        "FAQuery/ID",
                        "The identification must be unique and not already exist")]
        public bool FAL03000650(FLUXFAQueryMessageType query)
        {
            if (query.FAQuery != null && query.FAQuery.ID != null && !string.IsNullOrEmpty(query.FAQuery.ID.Value))
            {
                bool exists = (from fvms in Db.Fluxfvmsrequests
                               where fvms.RequestUuid.ToString() == query.FAQuery.ID.Value
                               select 1).Any();

                return !exists;
            }

            return true;
        }
    }
}
