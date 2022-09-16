using System.Collections.Generic;
using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class ParametersModification
    {
        [XmlArray(ElementName = "reportParameters")]
        [XmlArrayItem(ElementName = "reportParameter")]
        public List<ReportParameter> ReportParameters { get; set; }
    }
}
