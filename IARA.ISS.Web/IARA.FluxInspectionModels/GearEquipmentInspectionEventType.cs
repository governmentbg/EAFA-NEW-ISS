namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GearEquipmentInspectionEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GearEquipmentInspectionEventType
    {

        private IDType idField;

        private DateTimeType occurrenceDateTimeField;

        private GearCharacteristicType[] applicableGearCharacteristicField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public DateTimeType OccurrenceDateTime
        {
            get
            {
                return this.occurrenceDateTimeField;
            }
            set
            {
                this.occurrenceDateTimeField = value;
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
    }
}