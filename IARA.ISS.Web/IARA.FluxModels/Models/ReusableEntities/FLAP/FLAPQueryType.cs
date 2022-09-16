using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:18")]
    public partial class FLAPQueryType
    {
        public DateTimeType SubmittedDateTime { get; set; }


        public CodeType TypeCode { get; set; }


        public DelimitedPeriodType SpecifiedDelimitedPeriod { get; set; }


        [XmlElement("SubjectFLAPIdentity")]
        public FLAPIdentityType[] SubjectFLAPIdentity { get; set; }


        [XmlElement("SubjectVesselIdentity")]
        public VesselIdentityType[] SubjectVesselIdentity { get; set; }


        public FLUXPartyType SubmitterFLUXParty { get; set; }
    }
}
