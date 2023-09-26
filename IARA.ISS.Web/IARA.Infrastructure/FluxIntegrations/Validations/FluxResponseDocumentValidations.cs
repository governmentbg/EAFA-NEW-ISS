using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Validations.Common;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations
{
    public class FluxResponseDocumentValidations : FluxValidations<FLUXResponseDocumentType>
    {
        public FluxResponseDocumentValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider)
            : base(db, serviceProvider)
        { }

        [FluxValidation("FA-L03-00-0382",
                        "L03",
                        FluxValidationSeverity.Error,
                        "FLUXResponseDocument/ID",
                        "The identification must be unique and not already exist")]
        public bool FAL03000382(FLUXResponseDocumentType response)
        {
            if (response.ID != null && response.ID.Length != 0 && !string.IsNullOrEmpty(response.ID[0].Value))
            {
                bool exists = (from fvms in Db.Fluxfvmsrequests
                               where fvms.ResponseUuid.HasValue && fvms.ResponseUuid.ToString() == response.ID[0].Value
                               select 1).Any();

                return !exists;
            }

            return true;
        }

        [FluxValidation("FA-L03-00-0385",
                        "L03",  
                        FluxValidationSeverity.Warning,
                        "FLUXResponseDocument/ReferencedID",
                        "The identification must exist for a FLUXFAReportMessage or for a FLUXFAQuery message")]
        public bool FAL03000385(FLUXResponseDocumentType response)
        {
            if (response.ReferencedID != null && !string.IsNullOrEmpty(response.ReferencedID.Value))
            {
                bool exists = (from fvms in Db.Fluxfvmsrequests
                               where fvms.RequestUuid.ToString() == response.ID[0].Value
                               select 1).Any();

                return exists;
            }

            return true;
        }
    }
}
