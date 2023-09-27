namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTProductProcessEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTProductProcessEventType
    {

        private IDType locationIDField;

        private CodeType typeCodeField;

        private DateTimeType occurrenceDateTimeField;

        private IDType[] outputObjectInstanceIDField;

        private CodeType businessStepCodeField;

        private TTLocationType[] relatedTTLocationField;

        private TechnicalCharacteristicType[] relatedTechnicalCharacteristicField;

        public IDType LocationID
        {
            get
            {
                return this.locationIDField;
            }
            set
            {
                this.locationIDField = value;
            }
        }

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

        [XmlElement("OutputObjectInstanceID")]
        public IDType[] OutputObjectInstanceID
        {
            get
            {
                return this.outputObjectInstanceIDField;
            }
            set
            {
                this.outputObjectInstanceIDField = value;
            }
        }

        public CodeType BusinessStepCode
        {
            get
            {
                return this.businessStepCodeField;
            }
            set
            {
                this.businessStepCodeField = value;
            }
        }

        [XmlElement("RelatedTTLocation")]
        public TTLocationType[] RelatedTTLocation
        {
            get
            {
                return this.relatedTTLocationField;
            }
            set
            {
                this.relatedTTLocationField = value;
            }
        }

        [XmlElement("RelatedTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] RelatedTechnicalCharacteristic
        {
            get
            {
                return this.relatedTechnicalCharacteristicField;
            }
            set
            {
                this.relatedTechnicalCharacteristicField = value;
            }
        }
    }
}