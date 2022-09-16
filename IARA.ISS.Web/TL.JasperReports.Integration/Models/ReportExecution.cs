using System.Collections.Generic;
using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class ReportExecution
    {
        [XmlElement(ElementName = "currentPage")]
        public int? CurrentPage { get; set; }

        [XmlElement(ElementName = "reportURI")]
        public string ReportURI { get; set; }

        [XmlElement(ElementName = "requestId")]
        public string RequestId { get; set; }

        [XmlElement(ElementName = "status")]
        public string Status { get; set; }

        [XmlArray(ElementName = "exports")]
        [XmlArrayItem(ElementName = "export")]
        public List<ExportExecution> Exports { get; set; }
    }
}
