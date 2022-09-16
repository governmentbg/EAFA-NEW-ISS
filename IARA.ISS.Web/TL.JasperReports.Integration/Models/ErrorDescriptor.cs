using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class ErrorDescriptor
    {
        [XmlElement(ElementName = "errorCode")]
        public string ErrorCode { get; set; }

        [XmlElement(ElementName = "message")]
        public string Message { get; set; }

        [XmlArray(ElementName = "parameters")]
        [XmlArrayItem(ElementName = "parameter")]
        public List<string> Parameters { get; set; }
    }
}
