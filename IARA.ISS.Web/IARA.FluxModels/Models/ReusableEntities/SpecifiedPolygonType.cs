using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SpecifiedPolygonType
    {
        [XmlElement("InteriorSpecifiedLinearRing")]
        public SpecifiedLinearRingType[] InteriorSpecifiedLinearRing { get; set; }

        public SpecifiedLinearRingType ExteriorSpecifiedLinearRing { get; set; }

        public SpecifiedGeographicalObjectCharacteristicType AssociatedSpecifiedGeographicalObjectCharacteristic { get; set; }
    }
}
