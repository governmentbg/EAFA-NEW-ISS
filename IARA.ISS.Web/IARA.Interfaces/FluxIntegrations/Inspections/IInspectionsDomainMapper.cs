using IARA.FluxInspectionModels;
using IARA.FluxModels.Enums;
using System;

namespace IARA.Interfaces.FluxIntegrations.Inspections
{
    public interface IInspectionsDomainMapper
    {
        FLUXISRMessageType MapOFSToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId);

        FLUXISRMessageType MapIBSToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId);

        FLUXISRMessageType MapIBPToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId);

        FLUXISRMessageType MapITBToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId);

        FLUXISRMessageType MapIVHToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId);

        FLUXISRMessageType MapIFSToInspectionReport(int id, ReportPurposeCodes purpose, Guid referenceId);
    }
}
