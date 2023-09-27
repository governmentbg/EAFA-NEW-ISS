using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Infrastructure.FluxIntegrations.Validations.Common;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations
{
    public class FluxVesselQueryValidations : FluxValidations<FLUXVesselQueryMessageType>
    {
        public FluxVesselQueryValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider)
            : base(db, serviceProvider)
        { }

        [FluxValidation("VESSEL-L00-03-9998",
                        "L00",
                        FluxValidationSeverity.Reject,
                        "FLUXVesselQueryMessage",
                        "The query returns no results")]
        public bool VESSELL00039998(FLUXVesselQueryMessageType query)
        {
            IVesselsDomainService service = ServiceProvider.GetService<IVesselsDomainService>();
            return service.QueryHasResults(query);
        }

        [FluxValidation("VESSEL-L00-03-0102",
                        "L00",
                        FluxValidationSeverity.Reject,
                        "FLUXVesselQueryMessage",
                        "The identification must be unique and not already exist",
                        true)]
        public bool VESSEL00030102(FLUXVesselQueryMessageType query)
        {
            if (query.VesselQuery != null && query.VesselQuery.ID != null && !string.IsNullOrEmpty(query.VesselQuery.ID.Value))
            {
                bool exists = (from fvms in Db.Fluxfvmsrequests
                               where fvms.RequestUuid.ToString() == query.VesselQuery.ID.Value
                               select 1).Any();

                return !exists;
            }

            return true;
        }
    }
}
