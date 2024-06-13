using IARA.FluxInspectionModels;
using IARA.Infrastructure.FluxIntegrations.Validations.Common;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations
{
    public class FluxInspectionsReportValidations : FluxValidations<FLUXISRMessageType>
    {
        public FluxInspectionsReportValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider)
            : base(db, serviceProvider)
        { }
    }
}
