using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SpecifiedGeographicalObjectCharacteristicType
    {
        public IDType ID { get; set; }

        public TextType Description { get; set; }

        public TextType DescriptionReference { get; set; }

        public TextType Name { get; set; }
    }
}
