using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselQueryType
    {
        public DateTimeType SubmittedDateTime { get; set; }


        public CodeType TypeCode { get; set; }


        public IDType ID { get; set; }


        public FLUXPartyType SubmitterFLUXParty { get; set; }


        [XmlElement("SimpleVesselQueryParameter")]
        public VesselQueryParameterType[] SimpleVesselQueryParameter { get; set; }


        [XmlElement("CompoundLogicalQueryParameter")]
        public LogicalQueryParameterType[] CompoundLogicalQueryParameter { get; set; }


        public VesselIdentityType SubjectVesselIdentity { get; set; }


        public DelimitedPeriodType SpecifiedDelimitedPeriod { get; set; }
    }
}
