using System.Collections.Generic;
using System.Xml.Serialization;

namespace TL.JasperReports.Integration
{
    public class ReportExecutionRequest
    {
        public ReportExecutionRequest()
        {
            this.FreshData = false;
            this.SaveDataSnapshot = false;
            this.Interactive = true;
            this.AllowInlineScripts = true;
        }

        /// <summary>
        /// Repository path (URI) of the report to run. For commercial editions with organizations, the URI is relative to the logged-in user’s organization
        /// </summary>
        [XmlElement(ElementName = "reportUnitUri")]
        public string ReportUnitUri { get; set; }

        /// <summary>
        /// Determines whether reportExecution is synchronous or asynchronous. When set to true, 
        /// the response is sent immediately and the client must poll the report status and later download the result when ready. 
        /// By default, this property is false and the operation will wait until the report execution is complete, 
        /// forcing the client to wait as well, but allowing the client to download the report immediately after the response.
        /// </summary>
        [XmlElement(ElementName = "async")]
        public bool Async { get; set; }

        /// <summary>
        /// When data snapshots are enabled, specifies whether the report should get fresh data by querying the data source or if false, 
        /// use a previously saved data snapshot (if any). 
        /// By default, if a saved data snapshot exists for the report it will be used when running the report
        /// </summary>
        [XmlElement(ElementName = "freshData")]
        public bool FreshData { get; set; }

        /// <summary>
        /// When data snapshots are enabled, specifies whether the data snapshot for the report should be written or overwritten with the new data from this execution of the report
        /// </summary>
        [XmlElement(ElementName = "saveDataSnapshot")]
        public bool SaveDataSnapshot { get; set; }

        /// <summary>
        /// Specifies the desired output format: pdf, html, xls, xlsx, rtf, csv, xml, docx, odt, ods, jrprint.
        /// </summary>
        [XmlElement(ElementName = "outputFormat")]
        public string OutputFormat { get; set; }

        /// <summary>
        /// In a commercial editions of the server where HighCharts are used in the report, 
        /// this property determines whether the JavaScript necessary for interaction is generated and returned as an attachment when exporting to HTML. 
        /// If false, the chart is generated as a non-interactive image file (also as an attachment).
        /// </summary>
        [XmlElement(ElementName = "interactive")]
        public bool Interactive { get; set; }

        /// <summary>
        /// When set to true, the report is generated as a single long page. 
        /// This can be used with HTML output to avoid pagination. 
        /// When omitted, the ignorePagination property on the JRXML, if any, is used
        /// </summary>
        [XmlElement(ElementName = "ignorePagination")]
        public bool IgnorePagination { get; set; }

        /// <summary>
        /// Specify a page range to generate a partial report. The format is:<startPageNumber>-<endPageNumber>
        /// </summary>
        [XmlElement(ElementName = "pages")]
        public string Pages { get; set; }

        /// <summary>
        /// A list of input control parameters and their values.
        /// </summary>
        [XmlArray(ElementName = "parameters")]
        [XmlArrayItem(ElementName = "reportParameter")]
        public List<ReportParameter> Parameters { get; set; }

        /// <summary>
        /// Affects HTML export only. If true, then inline scripts are allowed, otherwise no inline script is included in the HTML output.
        /// </summary>
        [XmlElement(ElementName = "allowInlineScripts")]
        public bool AllowInlineScripts { get; set; }


        /// <summary>
        /// For HTML output, this property specifies the URL path to use for downloading the attachment files (JavaScript and images). 
        /// The full path of the default value is: {contextPath}/rest_v2/reportExecutions/{reportExecutionId}/exports/{exportExecutionId}/attachments/
        /// You can specify a different URL path using the placeholders { contextPath}, { reportExecutionId}, and {exportExecutionId}.
        /// </summary>
        [XmlElement(ElementName = "attachmentsPrefix")]
        public string AttachmentsPrefix { get; set; }

        [XmlElement(ElementName = "baseUrl")]
        public string BaseUrl { get; set; }
    }
}
