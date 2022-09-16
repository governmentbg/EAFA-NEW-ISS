using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using TL.JasperReports.Integration.Enums;

namespace TL.JasperReports.Integration.Models
{
    public class Export
    {
        [XmlElement(ElementName = "outputFormat")]
        public OutputFormats OuputFormat { get; set; }

        [XmlElement(ElementName = "pages")]
        public string Pages { get; set; }

        [XmlElement(ElementName = "attachmentsPrefix")]
        public string AttachmentsPrefix { get; set; }
    }
}
