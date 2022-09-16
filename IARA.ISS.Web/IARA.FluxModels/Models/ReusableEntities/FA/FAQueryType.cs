using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FAQueryType
    {
        public IDType ID { get; set; }


        public DateTimeType SubmittedDateTime { get; set; }


        public CodeType TypeCode { get; set; }


        public DelimitedPeriodType SpecifiedDelimitedPeriod { get; set; }


        public FLUXPartyType SubmitterFLUXParty { get; set; }


        [XmlElement("SimpleFAQueryParameter")]
        public FAQueryParameterType[] SimpleFAQueryParameter { get; set; }
    }
}
