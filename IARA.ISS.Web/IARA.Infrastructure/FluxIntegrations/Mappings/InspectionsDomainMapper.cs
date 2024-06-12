using IARA.FluxInspectionModels;
using IARA.FluxModels.Enums;
using IARA.Interfaces.FluxIntegrations.Inspections;

namespace IARA.Infrastructure.FluxIntegrations.Mappings
{
    public class InspectionsDomainMapper : BaseService, IInspectionsDomainMapper
    {
        public InspectionsDomainMapper(IARADbContext dbContext)
            : base(dbContext)
        { }

        public FLUXISRMessageType MapOFSToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId)
        {
            FLUXISRMessageType message = new FLUXISRMessageType(); //TODO 
            return message;
        }

        public FLUXISRMessageType MapIBSToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId)
        {
            FLUXISRMessageType message = new FLUXISRMessageType(); //TODO
            return message;
        }

        public FLUXISRMessageType MapIBPToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId)
        {
            FLUXISRMessageType message = new FLUXISRMessageType(); //TODO
            return message;
        }

        public FLUXISRMessageType MapITBToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId)
        {
            FLUXISRMessageType message = new FLUXISRMessageType(); //TODO
            return message;
        }

        public FLUXISRMessageType MapIVHToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId)
        {
            FLUXISRMessageType message = new FLUXISRMessageType(); //TODO
            return message;
        }

        public FLUXISRMessageType MapIFSToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId)
        {
            FLUXISRMessageType message = new FLUXISRMessageType(); //TODO
            return message;
        }
    }
}
