using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FishingGearEquipmentType
    {
        public CodeType TypeCode { get; set; }

        [XmlElement("ApplicableFLUXCharacteristic")]
        public FLUXCharacteristicType[] ApplicableFLUXCharacteristic { get; set; }
    }
}
