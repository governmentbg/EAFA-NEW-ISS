using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FLUXResponseDocumentType
    {
        [XmlElement("ID")]
        public IDType[] ID { get; set; }

        public IDType ReferencedID { get; set; }

        public DateTimeType CreationDateTime { get; set; }

        public CodeType ResponseCode { get; set; }

        public TextType Remarks { get; set; }

        public TextType RejectionReason { get; set; }

        public CodeType TypeCode { get; set; }


        [XmlElement("RelatedValidationResultDocument")]
        public ValidationResultDocumentType[] RelatedValidationResultDocument { get; set; }

        public FLUXPartyType RespondentFLUXParty { get; set; }
    }
}
