using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FishingTripType
    {
        [XmlElement("ID")]
        public IDType[] ID { get; set; }


        public CodeType TypeCode { get; set; }


        [XmlElement("SpecifiedDelimitedPeriod")]
        public DelimitedPeriodType[] SpecifiedDelimitedPeriod { get; set; }
    }
}
