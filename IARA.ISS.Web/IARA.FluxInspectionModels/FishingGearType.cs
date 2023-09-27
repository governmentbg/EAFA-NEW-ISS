namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FishingGear", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FishingGearType
    {

        private CodeType typeCodeField;

        private CodeType[] roleCodeField;

        private FLUXPictureType[] illustrateFLUXPictureField;

        private GearCharacteristicType[] applicableGearCharacteristicField;

        private GearInspectionEventType[] relatedGearInspectionEventField;

        private FishingGearEquipmentType[] attachedFishingGearEquipmentField;

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        [XmlElement("RoleCode")]
        public CodeType[] RoleCode
        {
            get
            {
                return this.roleCodeField;
            }
            set
            {
                this.roleCodeField = value;
            }
        }

        [XmlElement("IllustrateFLUXPicture")]
        public FLUXPictureType[] IllustrateFLUXPicture
        {
            get
            {
                return this.illustrateFLUXPictureField;
            }
            set
            {
                this.illustrateFLUXPictureField = value;
            }
        }

        [XmlElement("ApplicableGearCharacteristic")]
        public GearCharacteristicType[] ApplicableGearCharacteristic
        {
            get
            {
                return this.applicableGearCharacteristicField;
            }
            set
            {
                this.applicableGearCharacteristicField = value;
            }
        }

        [XmlElement("RelatedGearInspectionEvent")]
        public GearInspectionEventType[] RelatedGearInspectionEvent
        {
            get
            {
                return this.relatedGearInspectionEventField;
            }
            set
            {
                this.relatedGearInspectionEventField = value;
            }
        }

        [XmlElement("AttachedFishingGearEquipment")]
        public FishingGearEquipmentType[] AttachedFishingGearEquipment
        {
            get
            {
                return this.attachedFishingGearEquipmentField;
            }
            set
            {
                this.attachedFishingGearEquipmentField = value;
            }
        }
    }
}