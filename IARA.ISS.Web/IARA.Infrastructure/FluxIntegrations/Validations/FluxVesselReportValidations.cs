using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Validations.Common;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations
{
    public class FluxVesselReportValidations : FluxValidations<FLUXReportVesselInformationType>
    {
        public FluxVesselReportValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider)
            : base(db, serviceProvider)
        { }

        [FluxValidation("VESSEL-L00-00-0003",
                       "L00",
                       FluxValidationSeverity.Reject,
                       "FLUX_Report Document/Identification",
                       "The identification must be unique and not already exist",
                       true)]
        public bool VESSEL00000003(FLUXReportVesselInformationType report)
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
