using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TL.JasperReports.Integration
{
    internal static class URLS
    {
        private const string DATETIME_FORMAT = "yyyy-MM-dd'T'HH:mmZ";

        public const string RUN_REPORT_ASYNC = "/rest_v2/reportExecutions";

        public static readonly Func<string, string> POLLING_REPORT_EXECUTION = requestId => $"/rest_v2/reportExecutions/{requestId}/status/";
        public static readonly Func<string, string> REPORT_EXECUTION_DETAILS = requestId => $"/rest_v2/reportExecutions/{requestId}";
        public static readonly Func<string, string, string> REQUEST_REPORT_OUTPUT = (requestId, exportId) => $"/rest_v2/reportExecutions/{requestId}/exports/{exportId}/outputResource";
        public static readonly Func<string, string> EXPORT_REPORT_ASYNC = (requestId) => $"/rest_v2/reportExecutions/{requestId}/exports/";
        public static readonly Func<string, string> MODIFY_REPORT_PARAMETERS = (requestId) => $"/rest_v2/reportExecutions/{requestId}/parameters";
        public static readonly Func<string, string, string> POLLING_REPORT_EXPORT = (requestId, exportId) => $"/rest_v2/reportExecutions/{requestId}/exports/{exportId}/status";
        public static readonly Func<string, string> STOP_RUNNING_REPORTS = (requestId) => $"/rest_v2/reportExecutions/{requestId}/status/";
        public static readonly Func<string, string> LIST_INPUT_CONTROLS = (pathToReport) => $"/rest_v2/reports/{pathToReport}/inputControls/";
        public static readonly Func<string, string> LIST_INPUT_CONTROL_VALUES = (pathToReport) => $"/rest_v2/reports/{pathToReport}/inputControls/values/";
        public static readonly Func<string, string> SETTING_INPUT_CONTROL_VALUES = (pathToReport) => $"/rest_v2/reports/{pathToReport}/inputControls/";
        public static string FIND_RUNNING_REPORTS(string reportURI = null, string jobID = null, string jobLabel = null, string userName = null, DateTime? fireTimeFrom = null, DateTime? fireTimeTo = null)
        {
            string url = $"/rest_v2/reportExecutions?";
            StringBuilder builder = new StringBuilder();
            builder.Append(url);


            if (!string.IsNullOrEmpty(reportURI))
            {
                builder.AppendJoin('&', $"{nameof(reportURI)}={reportURI}");
            }


            if (!string.IsNullOrEmpty(jobID))
            {
                builder.AppendJoin('&', $"{nameof(jobID)}={jobID}");
            }

            if (!string.IsNullOrEmpty(jobLabel))
            {
                builder.AppendJoin('&', $"{nameof(jobLabel)}={jobLabel}");
            }

            if (!string.IsNullOrEmpty(userName))
            {
                builder.AppendJoin('&', $"{nameof(userName)}={userName}");
            }

            if (fireTimeFrom != null && fireTimeTo != null)
            {
                builder.AppendJoin('&', $"{nameof(fireTimeFrom)}={fireTimeFrom.Value.ToString(DATETIME_FORMAT)}");
                builder.AppendJoin('&', $"{nameof(fireTimeTo)}={fireTimeTo.Value.ToString(DATETIME_FORMAT)}");
            }

            return builder.ToString();
        }

        public static string RUN_REPORT(string pathToReport, string format, Dictionary<string, string> inputControls = null, uint? page = null, bool? ignorePagination = null, bool? interactive = null, bool? onePagePerSheet = null)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("/rest_v2/reports/");
            builder.Append(pathToReport.TrimStart('/'));
            builder.Append(".");
            builder.Append(format);

            bool addedQuestionMark = false;

            if (inputControls != null && inputControls.Any())
            {
                builder.Append("?");
                addedQuestionMark = true;
                builder.AppendJoin('&', inputControls.Select(x => $"{x.Key}={x.Value}"));
            }

            if (page.HasValue)
            {
                if (!addedQuestionMark)
                {
                    builder.Append("?");
                    addedQuestionMark = true;
                }

                builder.AppendJoin('&', $"{nameof(page)}={page}");
            }

            if (ignorePagination.HasValue)
            {
                if (!addedQuestionMark)
                {
                    builder.Append("?");
                    addedQuestionMark = true;
                }

                builder.AppendJoin('&', $"{nameof(ignorePagination)}={ignorePagination}");
            }

            if (interactive.HasValue)
            {
                if (!addedQuestionMark)
                {
                    builder.Append("?");
                    addedQuestionMark = true;
                }

                builder.AppendJoin('&', $"{nameof(interactive)}={interactive}");
            }

            if (onePagePerSheet.HasValue)
            {
                if (!addedQuestionMark)
                {
                    builder.Append("?");
                    addedQuestionMark = true;
                }

                builder.AppendJoin('&', $"{nameof(onePagePerSheet)}={onePagePerSheet}");
            }

            return builder.ToString();
        }

        public static string RUN_REPORT(string pathToReport, string format, object inputControls = null, uint? page = null, bool? ignorePagination = null, bool? interactive = null, bool? onePagePerSheet = null)
        {
            Dictionary<string, string> inputControlsDictionary = new Dictionary<string, string>();
            if (inputControls != null)
            {
                var properties = inputControls.GetType().GetProperties();

                foreach (var property in properties)
                {
                    inputControlsDictionary.Add(property.Name, property.GetValue(inputControls)?.ToString());
                }
            }

            return RUN_REPORT(pathToReport, format, inputControlsDictionary, page, ignorePagination, interactive, onePagePerSheet);
        }

    }
}
