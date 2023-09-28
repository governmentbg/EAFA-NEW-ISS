namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GearInspectionEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GearInspectionEventType
    {

        private IDType[] idField;

        private TextType[] descriptionField;

        private DateTimeType occurrenceDateTimeField;

        private DelimitedPeriodType occurrenceDelimitedPeriodField;

        private GearCharacteristicType[] relatedGearCharacteristicField;

        [XmlElement("ID")]
        public IDType[] ID
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

        [XmlElement("Description")]
        public TextType[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
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

        public DelimitedPeriodType OccurrenceDelimitedPeriod
        {
            get
            {
                return this.occurrenceDelimitedPeriodField;
            }
            set
            {
                this.occurrenceDelimitedPeriodField = value;
            }
        }

        [XmlElement("RelatedGearCharacteristic")]
        public GearCharacteristicType[] RelatedGearCharacteristic
        {
            get
            {
                return this.relatedGearCharacteristicField;
            }
            set
            {
                this.relatedGearCharacteristicField = value;
            }
        }
    }
}