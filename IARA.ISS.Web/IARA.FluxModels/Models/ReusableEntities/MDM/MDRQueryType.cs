using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class MDRQueryType
    {
        public IDType ID { get; set; }

        public DateTimeType SubmittedDateTime { get; set; }

        public CodeType TypeCode { get; set; }

        public CodeType ContractualLanguageCode { get; set; }

        public FLUXPartyType SubmitterFLUXParty { get; set; }

        public MDRQueryIdentityType SubjectMDRQueryIdentity { get; set; }
    }
}
