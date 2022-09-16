using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TL.JasperReports.Integration.Enums;

namespace TL.JasperReports.Integration
{
    public interface IJasperReportsClient : IDisposable
    {
        Task<Stream> RunParameterlessReport(string pathToReport, OutputFormats format, uint? page = null, bool? ignorePagination = null, bool? interactive = null, bool? onePagePerSheet = null);
        Task<Stream> RunReport(string pathToReport, OutputFormats format, Dictionary<string, string> inputs, uint? page = null, bool? ignorePagination = null, bool? interactive = null, bool? onePagePerSheet = null);
        Task<Stream> RunReport<T>(string pathToReport, OutputFormats format, T inputs, uint? page = null, bool? ignorePagination = null, bool? interactive = null, bool? onePagePerSheet = null);
        Task<byte[]> RunReportBuffered<T>(string pathToReport, OutputFormats format, T inputs, uint? page = null, bool? ignorePagination = null, bool? interactive = null, bool? onePagePerSheet = null);
        Task<byte[]> RunReportBuffered(string pathToReport, OutputFormats format, Dictionary<string, string> inputs, uint? page = null, bool? ignorePagination = null, bool? interactive = null, bool? onePagePerSheet = null);
    }
}
