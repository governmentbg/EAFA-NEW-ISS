using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using TL.JasperReports.Integration.Enums;
using TL.JasperReports.Integration.Models;

namespace TL.JasperReports.Integration
{
    public class JasperReportsClient : IJasperReportsClient, IJasperReportExecutionsClient, IJasperInputControlsClient
    {
        private const string XML_CONTENT_TYPE = "application/xml";
        private const string JSON_CONTENT_TYPE = "application/json";
        private const string XML_CONTENT_STATUS = "accept: application/status+xml";
        private const string JSON_CONTENT_STATUS = "accept: application/status+json";

        private HttpClient httpClient;
        private bool disposedValue;
        private readonly ContentType type;
        private readonly string baseJasperServerPath;

        public JasperReportsClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            JasperSettings.LoadSettings(configuration);
            httpClient = httpClientFactory.CreateClient();
            this.type = JasperSettings.Default.Type;
            httpClient.BaseAddress = new Uri($"{JasperSettings.Default.Scheme}://{JasperSettings.Default.Host}:{JasperSettings.Default.Port}");
            baseJasperServerPath = JasperSettings.Default.BaseJasperServerPath;
            httpClient.DefaultRequestHeaders.Accept.Clear();

            string token = JasperSettings.Default.GetBasicAuth();

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            }

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(XML_CONTENT_TYPE));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_CONTENT_TYPE));
        }

        public async Task<byte[]> RunReportBuffered<T>(string pathToReport,
                                       OutputFormats format,
                                       T inputs,
                                       uint? page = null,
                                       bool? ignorePagination = null,
                                       bool? interactive = null,
                                       bool? onePagePerSheet = null)
        {
            Stream stream = await RunReport<T>(pathToReport, format, inputs, page, ignorePagination, interactive, onePagePerSheet);

            MemoryStream memStream = new MemoryStream();

            stream.CopyTo(memStream);
            stream.Flush();
            return memStream.ToArray();
        }

        public async Task<byte[]> RunReportBuffered(string pathToReport,
                                       OutputFormats format,
                                       Dictionary<string, string> inputs,
                                       uint? page = null,
                                       bool? ignorePagination = null,
                                       bool? interactive = null,
                                       bool? onePagePerSheet = null)
        {
            Stream stream = await RunReport(pathToReport, format, inputs, page, ignorePagination, interactive, onePagePerSheet);

            MemoryStream memStream = new MemoryStream();

            stream.CopyTo(memStream);
            stream.Flush();
            return memStream.ToArray();
        }

        public async Task<Stream> RunReport(string pathToReport,
                                   OutputFormats format,
                                   Dictionary<string, string> inputs,
                                   uint? page = null,
                                   bool? ignorePagination = null,
                                   bool? interactive = null,
                                   bool? onePagePerSheet = null)
        {
            HttpResponseMessage response = await GetAsync(URLS.RUN_REPORT(pathToReport, format.ToString(), inputs, page, ignorePagination, interactive, onePagePerSheet));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> RunReport<T>(string pathToReport,
                               OutputFormats format,
                               T inputs,
                               uint? page = null,
                               bool? ignorePagination = null,
                               bool? interactive = null,
                               bool? onePagePerSheet = null)
        {
            HttpResponseMessage response = await GetAsync(URLS.RUN_REPORT(pathToReport, format.ToString(), inputs, page, ignorePagination, interactive, onePagePerSheet));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }

        public Task<Stream> RunParameterlessReport(string pathToReport, OutputFormats format, uint? page = null, bool? ignorePagination = null, bool? interactive = null, bool? onePagePerSheet = null)
        {
            return RunReport<object>(pathToReport, format, null, page, ignorePagination, interactive, onePagePerSheet);
        }

        public async Task<ReportExecution> RunAsyncReport(ReportExecutionRequest request)
        {
            StringContent requestContent = type == ContentType.XML ? ToXmlContent(request) : ToJsonContent(request);

            HttpResponseMessage response = await PostAsync(URLS.RUN_REPORT_ASYNC, requestContent);
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();

            ReportExecution report = type == ContentType.XML ? FromXmlContent<ReportExecution>(body) : FromJsonContent<ReportExecution>(body);

            return report;
        }

        public async Task<string> PollReportStatus(string requestId)
        {
            HttpResponseMessage response = await GetAsync(URLS.POLLING_REPORT_EXECUTION(requestId));
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();

            SimpleStatus result = type == ContentType.XML ? FromXmlContent<SimpleStatus>(body) : FromJsonContent<SimpleStatus>(body);

            return result.Status;
        }

        public async Task<string> GetExecutionDetails(string requestId)
        {
            HttpResponseMessage response = await GetAsync(URLS.REPORT_EXECUTION_DETAILS(requestId));
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            SimpleStatus result = type == ContentType.XML ? FromXmlContent<SimpleStatus>(body) : FromJsonContent<SimpleStatus>(body);

            return result.Status;
        }

        public async Task<Stream> GetReportOutput(string requestId, string exportId)
        {
            HttpResponseMessage response = await GetAsync(URLS.REQUEST_REPORT_OUTPUT(requestId, exportId));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }


        public async Task<ExportExecution> ExportAsyncReport(string requestId, Export export)
        {
            StringContent requestContent = type == ContentType.XML ? ToXmlContent(export) : ToJsonContent(export);
            HttpResponseMessage response = await PostAsync(URLS.EXPORT_REPORT_ASYNC(requestId), requestContent);
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            ExportExecution execution = type == ContentType.XML ? FromXmlContent<ExportExecution>(body) : FromJsonContent<ExportExecution>(body);

            return execution;
        }

        //public async Task ModifyReportParameters()

        public async Task<string> PollExportExecution(string requestId, string exportId)
        {
            HttpResponseMessage response = await GetAsync(URLS.POLLING_REPORT_EXPORT(requestId, exportId));
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            SimpleStatus result = type == ContentType.XML ? FromXmlContent<SimpleStatus>(body) : FromJsonContent<SimpleStatus>(body);

            return result.Status;
        }

        public async Task<List<ReportExecution>> GetAllRunningReports(string reportURI = null,
                                                                      string jobID = null,
                                                                      string jobLabel = null,
                                                                      string userName = null,
                                                                      DateTime? fireTimeFrom = null,
                                                                      DateTime? fireTimeTo = null)
        {
            string url = URLS.FIND_RUNNING_REPORTS(reportURI, jobID, jobLabel, userName, fireTimeFrom, fireTimeTo);
            HttpResponseMessage response = await GetAsync(url);
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            List<ReportExecution> reportExecutions = type == ContentType.XML ? FromXmlContent<List<ReportExecution>>(body) : FromJsonContent<List<ReportExecution>>(body);

            return reportExecutions;
        }

        public async Task<bool> StopRunningReport(string requestId)
        {
            SimpleStatus cancel = new SimpleStatus
            {
                Status = Statuses.Cancelled
            };

            StringContent requestContent = type == ContentType.XML ? ToXmlContent(cancel) : ToJsonContent(cancel);
            HttpResponseMessage response = await PutAsync(URLS.STOP_RUNNING_REPORTS(requestId), requestContent);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return false;
            }
            else
            {
                string body = await response.Content.ReadAsStringAsync();
                SimpleStatus result = type == ContentType.XML ? FromXmlContent<SimpleStatus>(body) : FromJsonContent<SimpleStatus>(body);
                return result.Status == Statuses.Cancelled;
            }
        }

        public async Task<InputControlsType> ListInputControlStructure(string pathToReport)
        {
            HttpResponseMessage response = await GetAsync(URLS.LIST_INPUT_CONTROLS(pathToReport));
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            InputControlsType result = FromJsonContent<InputControlsType>(body);
            return result;
        }

        public async Task<InputControlStateType> ListInputControlValues(string pathToReport)
        {
            HttpResponseMessage response = await GetAsync(URLS.LIST_INPUT_CONTROL_VALUES(pathToReport));
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            InputControlStateType result = FromJsonContent<InputControlStateType>(body);
            return result;
        }

        public async Task<InputControlStateType> SettingInputControlValues(string pathToReport, Dictionary<string, List<string>> inputControls)
        {
            StringContent request = ToJsonContent(inputControls);

            HttpResponseMessage response = await PostAsync(URLS.SETTING_INPUT_CONTROL_VALUES(pathToReport), request);
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            InputControlStateType result = FromJsonContent<InputControlStateType>(body);
            return result;
        }

        private T FromXmlContent<T>(string xml)
            where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xml);
            return serializer.Deserialize(reader) as T;
        }

        private T FromJsonContent<T>(string json)
        {
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            return JsonSerializer.Deserialize<T>(json, options);
        }

        private StringContent ToXmlContent<T>(T body)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            serializer.Serialize(writer, body);

            StringContent content = new StringContent(builder.ToString(), Encoding.UTF8, XML_CONTENT_TYPE);
            return content;
        }

        private StringContent ToJsonContent<T>(T body)
        {
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            string json = JsonSerializer.Serialize(body, options);
            StringContent content = new StringContent(json, Encoding.UTF8, JSON_CONTENT_TYPE);
            return content;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    httpClient = null;
                }

                disposedValue = true;
            }
        }

        private Task<HttpResponseMessage> GetAsync(string url)
        {
            string basePath = baseJasperServerPath.Trim('/');
            url = url.Trim('/');
            return httpClient.GetAsync($"/{basePath}/{url}");
        }

        private Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            string basePath = baseJasperServerPath.Trim('/');
            url = url.Trim('/');
            return httpClient.PostAsync($"/{basePath}/{url}", content);
        }

        private Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            string basePath = baseJasperServerPath.Trim('/');
            url = url.Trim('/');
            return httpClient.PutAsync($"/{basePath}/{url}", content);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
