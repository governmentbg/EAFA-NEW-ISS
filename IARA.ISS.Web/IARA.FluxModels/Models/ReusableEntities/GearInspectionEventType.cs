using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class GearInspectionEventType
    {
        [XmlElement("ID")]
        public IDType[] ID { get; set; }


        [XmlElement("Description")]
        public TextType[] Description { get; set; }


        public DateTimeType OccurrenceDateTime { get; set; }


        public DelimitedPeriodType OccurrenceDelimitedPeriod { get; set; }


        [XmlElement("RelatedGearCharacteristic")]
        public GearCharacteristicType[] RelatedGearCharacteristic { get; set; }
    }
}
