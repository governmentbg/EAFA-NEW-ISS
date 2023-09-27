namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnimalHoldingEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnimalHoldingEventType
    {

        private IDType locationIDField;

        private CodeType typeCodeField;

        private DateTimeType occurrenceDateTimeField;

        private AnimalMovementEventType arrivalSpecifiedAnimalMovementEventField;

        private AnimalMovementEventType departureSpecifiedAnimalMovementEventField;

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

        public AnimalMovementEventType ArrivalSpecifiedAnimalMovementEvent
        {
            get
            {
                return this.arrivalSpecifiedAnimalMovementEventField;
            }
            set
            {
                this.arrivalSpecifiedAnimalMovementEventField = value;
            }
        }

        public AnimalMovementEventType DepartureSpecifiedAnimalMovementEvent
        {
            get
            {
                return this.departureSpecifiedAnimalMovementEventField;
            }
            set
            {
                this.departureSpecifiedAnimalMovementEventField = value;
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