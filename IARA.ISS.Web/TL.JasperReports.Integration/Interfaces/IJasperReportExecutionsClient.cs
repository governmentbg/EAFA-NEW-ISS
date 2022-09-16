using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TL.JasperReports.Integration.Models;

namespace TL.JasperReports.Integration
{
    public interface IJasperReportExecutionsClient : IDisposable
    {
        Task<ExportExecution> ExportAsyncReport(string requestId, Export export);
        Task<List<ReportExecution>> GetAllRunningReports(string reportURI = null, string jobID = null, string jobLabel = null, string userName = null, DateTime? fireTimeFrom = null, DateTime? fireTimeTo = null);
        Task<string> GetExecutionDetails(string requestId);
        Task<Stream> GetReportOutput(string requestId, string exportId);
        Task<string> PollExportExecution(string requestId, string exportId);
        Task<string> PollReportStatus(string requestId);
        Task<ReportExecution> RunAsyncReport(ReportExecutionRequest request);
    }
}
