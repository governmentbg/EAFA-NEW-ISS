using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FishingGearType
    {
        public CodeType TypeCode { get; set; }


        [XmlElement("RoleCode")]
        public CodeType[] RoleCode { get; set; }


        [XmlElement("IllustrateFLUXPicture")]
        public FLUXPictureType[] IllustrateFLUXPicture { get; set; }


        [XmlElement("ApplicableGearCharacteristic")]
        public GearCharacteristicType[] ApplicableGearCharacteristic { get; set; }


        [XmlElement("RelatedGearInspectionEvent")]
        public GearInspectionEventType[] RelatedGearInspectionEvent { get; set; }


        [XmlElement("AttachedFishingGearEquipment")]
        public FishingGearEquipmentType[] AttachedFishingGearEquipment { get; set; }
    }
}
