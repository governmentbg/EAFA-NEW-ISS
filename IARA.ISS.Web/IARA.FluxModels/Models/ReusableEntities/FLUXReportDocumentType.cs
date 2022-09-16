using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FLUXReportDocumentType
    {
        [XmlElement("ID")]
        public IDType[] ID { get; set; }

        public IDType ReferencedID { get; set; }

        public DateTimeType CreationDateTime { get; set; }

        public CodeType PurposeCode { get; set; }

        public TextType Purpose { get; set; }

        public CodeType TypeCode { get; set; }

        public FLUXPartyType OwnerFLUXParty { get; set; }
    }
}
