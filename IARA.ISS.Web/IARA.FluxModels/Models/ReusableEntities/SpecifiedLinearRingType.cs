using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SpecifiedLinearRingType
    {
        [XmlElement("Coordinate")]
        public TextType[] Coordinate { get; set; }

        public SpecifiedDirectPositionType CoordinateSpecifiedDirectPosition { get; set; }

        [XmlElement("SpecifiedFLUXGeographicalCoordinate")]
        public FLUXGeographicalCoordinateType[] SpecifiedFLUXGeographicalCoordinate { get; set; }

        public SpecifiedGeographicalObjectCharacteristicType AssociatedSpecifiedGeographicalObjectCharacteristic { get; set; }
    }
}
