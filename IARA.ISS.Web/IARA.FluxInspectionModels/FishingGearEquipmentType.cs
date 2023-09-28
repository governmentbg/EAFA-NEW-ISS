namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FishingGearEquipment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FishingGearEquipmentType
    {

        private CodeType typeCodeField;

        private FLUXCharacteristicType[] applicableFLUXCharacteristicField;

        private GearEquipmentInspectionEventType[] relatedGearEquipmentInspectionEventField;

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

        [XmlElement("ApplicableFLUXCharacteristic")]
        public FLUXCharacteristicType[] ApplicableFLUXCharacteristic
        {
            get
            {
                return this.applicableFLUXCharacteristicField;
            }
            set
            {
                this.applicableFLUXCharacteristicField = value;
            }
        }

        [XmlElement("RelatedGearEquipmentInspectionEvent")]
        public GearEquipmentInspectionEventType[] RelatedGearEquipmentInspectionEvent
        {
            get
            {
                return this.relatedGearEquipmentInspectionEventField;
            }
            set
            {
                this.relatedGearEquipmentInspectionEventField = value;
            }
        }
    }
}