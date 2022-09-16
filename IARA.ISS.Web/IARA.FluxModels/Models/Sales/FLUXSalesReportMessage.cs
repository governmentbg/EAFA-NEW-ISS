using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXSalesReportMessage:3")]
    [XmlRoot("FLUXSalesReportMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXSalesReportMessage:3", IsNullable = false)]
    public partial class FLUXSalesReportMessageType
    {
        public FLUXReportDocumentType FLUXReportDocument { get; set; }


        [XmlElement("SalesReport")]
        public SalesReportType[] SalesReport { get; set; }
    }

}
