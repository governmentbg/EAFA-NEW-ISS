using System.Collections.Generic;
using IARA.DataAccess;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;

namespace IARA.Infrastructure.FluxIntegrations.Mappings
{
    public class FishingActivityToFluxFishingActivityReportMapper : BaseService
    {
        public FishingActivityToFluxFishingActivityReportMapper(IARADbContext dbContext)
            : base(dbContext)
        { }

        public FLUXFAReportMessageType MapFishingActivityToFluxDepartureNoFishOnBoard(List<ShipLogBookPage> shipPages)
        {
            FLUXFAReportMessageType faReportMessage = new FLUXFAReportMessageType();

            FLUXReportDocumentType fluxReportDocument = new FLUXReportDocumentType();

            // TODO

            FAReportDocumentType faReportDocument = new FAReportDocumentType();

            // TODO

            faReportMessage.FAReportDocument = new FAReportDocumentType[]
            {
                faReportDocument
            };

            return faReportMessage;
        }
    }
}
