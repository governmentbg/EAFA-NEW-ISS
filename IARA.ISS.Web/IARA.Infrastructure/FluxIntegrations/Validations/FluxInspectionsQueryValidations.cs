using IARA.FluxInspectionModels;
using IARA.Infrastructure.FluxIntegrations.Validations.Common;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations
{
    public class FluxInspectionsQueryValidations : FluxValidations<FLUXISRQueryMessageType>
    {
        public FluxInspectionsQueryValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider)
            : base(db, serviceProvider)
        { }

        public static void UpdateFluxResponse(FluxBusinessRuleTypes type, FLUXISRResponseMessageType response, List<FluxValidationResult> validations)
        {
            //UpdateFluxResponse(type, response.FLUXResponseDocument, validations); //TODO 
        }
    }
}
