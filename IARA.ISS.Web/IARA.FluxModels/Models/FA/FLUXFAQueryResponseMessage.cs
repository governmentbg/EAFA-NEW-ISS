using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFAQueryResponseMessage:1")]
    [XmlRoot("FLUXFAQueryResponseMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXFAQueryResponseMessage:1", IsNullable = false)]
    public partial class FLUXFAQueryResponseMessageType
    {
        public FLUXResponseDocumentType FLUXResponseDocument { get; set; }


        [XmlElement("FAReportDocument")]
        public FAReportDocumentType[] FAReportDocument { get; set; }
    }

}
