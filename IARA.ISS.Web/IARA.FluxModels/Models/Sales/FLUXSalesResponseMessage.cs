using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXSalesResponseMessage:3")]
    [XmlRoot("FLUXSalesResponseMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXSalesResponseMessage:3", IsNullable = false)]
    public partial class FLUXSalesResponseMessageType
    {
        public FLUXResponseDocumentType FLUXResponseDocument { get; set; }


        [XmlElement("SalesReport")]
        public SalesReportType[] SalesReport { get; set; }
    }
}
