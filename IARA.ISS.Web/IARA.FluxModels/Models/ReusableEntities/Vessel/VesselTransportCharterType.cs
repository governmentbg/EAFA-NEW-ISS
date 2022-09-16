using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselTransportCharterType
    {
        public CodeType TypeCode { get; set; }


        [XmlElement("ApplicableDelimitedPeriod")]
        public DelimitedPeriodType[] ApplicableDelimitedPeriod { get; set; }


        [XmlElement("SpecifiedContactParty")]
        public ContactPartyType[] SpecifiedContactParty { get; set; }
    }
}
