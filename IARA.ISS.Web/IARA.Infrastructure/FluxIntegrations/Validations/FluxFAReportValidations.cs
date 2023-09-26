using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Validations.Common;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations
{
    public class FluxFAReportValidations : FluxValidations<FLUXFAReportMessageType>
    {
        public FluxFAReportValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider)
            : base(db, serviceProvider)
        { }

        [FluxValidation("FA-L03-00-0004",
                        "L03",
                        FluxValidationSeverity.Error,
                        "FLUXFAReportMessage/FLUXReportDocument/ID",
                        "The identification must be unique and not already exist")]
        public bool FAL03000004(FLUXFAReportMessageType report)
        {
            if (report.FLUXReportDocument != null
                && report.FLUXReportDocument.ID != null
                && report.FLUXReportDocument.ID.Length != 0
                && !string.IsNullOrEmpty(report.FLUXReportDocument.ID[0].Value))
            {
                bool exists = (from fvms in Db.Fluxfvmsrequests
                               where fvms.RequestUuid.ToString() == report.FLUXReportDocument.ID[0].Value
                                    || (fvms.ResponseUuid.HasValue && fvms.ResponseUuid.Value.ToString() == report.FLUXReportDocument.ID[0].Value)
                               select 1).Any();

                return !exists;
            }

            return true;
        }

        [FluxValidation("FA-L03-00-0013",
                        "L03",
                        FluxValidationSeverity.Warning,
                        "FLUXFAReportMessage/FLUXReportDocument/ReferencedID",
                        "The identification must exist for a FLUXFAQueryMessage")]
        public bool FAL03000013(FLUXFAReportMessageType report)
        {
            if (report.FLUXReportDocument != null
                && report.FLUXReportDocument.ReferencedID != null
                && !string.IsNullOrEmpty(report.FLUXReportDocument.ReferencedID.Value))
            {
                bool exists = (from fvms in Db.Fluxfvmsrequests
                               where fvms.RequestUuid.ToString() == report.FLUXReportDocument.ReferencedID.Value
                               select 1).Any();

                return exists;
            }

            return true;
        }

        [FluxValidation("FA-L03-00-0038",
                        "L03",
                        FluxValidationSeverity.Warning,
                        "FAReportDocument/RelatedFLUXReportDocument/ReferencedID",
                        "Must be an identifier of an accepted report")]
        public bool FAL03000038(FLUXFAReportMessageType report)
        {
            if (report.FAReportDocument != null
                && report.FAReportDocument.Length != 0)
            {
                List<string> guids = new();

                foreach (FAReportDocumentType rep in report.FAReportDocument)
                {
                    if (rep.RelatedFLUXReportDocument != null
                        && rep.RelatedFLUXReportDocument.ReferencedID != null
                        && !string.IsNullOrEmpty(rep.RelatedFLUXReportDocument.ReferencedID.Value))
                    {
                        guids.Add(rep.RelatedFLUXReportDocument.ReferencedID.Value);
                    }
                }

                if (guids.Any())
                {
                    int count = (from fvms in Db.Fluxfvmsrequests
                                 where guids.Contains(fvms.RequestUuid.ToString())
                                 select 1).Count();

                    return count == guids.Count;
                }

                return true;
            }

            return true;
        }

        [FluxValidation("FA-L03-00-0062",
                        "L03",
                        FluxValidationSeverity.Warning,
                        "VesselTransportMeans/ID, RegistrationVesselCountry/ID",
                        "The vessel identification and registration location (flag state) must be consistent")]
        public bool FAL03000062(FLUXFAReportMessageType report)
        {
            return true;
        }

        [FluxValidation("FA-L03-00-0064",
                        "L03",
                        FluxValidationSeverity.Warning,
                        "VesselTransportMeans/ID, RegistrationVesselCountry/ID",
                        "If CFR, the vessel should be in the EU fleet register under the flag state at the report creation date")]
        public bool FAL03000064(FLUXFAReportMessageType report)
        {
            return true;
        }

        [FluxValidation("FA-L03-00-0065",
                        "L03",
                        FluxValidationSeverity.Warning,
                        "VesselTransportMeans/ID, RegistrationVesselCountry/ID",
                        "If ICCAT, the vessel should exist at report creation date in the register of ICCAT vessels (if available) under the flag state")]
        public bool FAL03000065(FLUXFAReportMessageType report)
        {
            return true;
        }

        [FluxValidation("FA-L03-00-0637",
                        "L03",
                        FluxValidationSeverity.Warning,
                        "VesselTransportMeans/ID, RegistrationVesselCountry/ID",
                        "If IRCS & External Marking are provided, the vessel should exist in the vessel register at report creation date under the flag state")]
        public bool FAL03000637(FLUXFAReportMessageType report)
        {
            return true;
        }

        [FluxValidation("FA-L03-00-0638",
                        "L03",
                        FluxValidationSeverity.Warning,
                        "VesselTransportMeans/ID, RegistrationVesselCountry/ID",
                        "If UVI is provided, the vessel should exist in the vessel register at report creation date under the flag state")]
        public bool FAL03000638(FLUXFAReportMessageType report)
        {
            return true;
        }
    }
}
