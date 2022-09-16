using System;
using System.Xml.Serialization;


namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFAReportMessage:3")]
    [XmlRoot("FLUXFAReportMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXFAReportMessage:3", IsNullable = false)]
    public partial class FLUXFAReportMessageType
    {
        public FLUXReportDocumentType FLUXReportDocument { get; set; }


        [XmlElement("FAReportDocument")]
        public FAReportDocumentType[] FAReportDocument { get; set; }
    }

}
