using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SalesReportType
    {
        public IDType ID { get; set; }

        public CodeType ItemTypeCode { get; set; }

        [XmlElement("IncludedSalesDocument")]
        public SalesDocumentType[] IncludedSalesDocument { get; set; }

        [XmlElement("IncludedValidationResultDocument")]
        public ValidationResultDocumentType[] IncludedValidationResultDocument { get; set; }
    }
}
