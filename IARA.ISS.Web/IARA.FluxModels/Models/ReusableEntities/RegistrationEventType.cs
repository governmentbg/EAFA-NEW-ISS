using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class RegistrationEventType
    {
        [XmlElement("Description")]
        public TextType[] Description { get; set; }

        public DateTimeType OccurrenceDateTime { get; set; }

        public RegistrationLocationType RelatedRegistrationLocation { get; set; }
    }
}
