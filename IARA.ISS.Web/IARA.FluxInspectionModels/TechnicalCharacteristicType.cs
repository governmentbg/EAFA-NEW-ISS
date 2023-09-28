namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TechnicalCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TechnicalCharacteristicType
    {

        private CodeType[] typeCodeField;

        private CodeType subordinateTypeCodeField;

        private MeasureType valueMeasureField;

        private TextType descriptionField;

        private CodeType measurementMethodCodeField;

        private IDType idField;

        private MeasureType capacityValueMeasureField;

        private CodeType certificationCodeField;

        private TextType licenceField;

        private DateTimeType constructionDateTimeField;

        private DateTimeType latestRenovationDateTimeField;

        private CodeType[] descriptionCodeField;

        private AnimalHoldingEventType[] specifiedAnimalHoldingEventField;

        private TTProductProcessEventType[] specifiedTTProductProcessEventField;

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
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

        public CodeType SubordinateTypeCode
        {
            get
            {
                return this.subordinateTypeCodeField;
            }
            set
            {
                this.subordinateTypeCodeField = value;
            }
        }

        public MeasureType ValueMeasure
        {
            get
            {
                return this.valueMeasureField;
            }
            set
            {
                this.valueMeasureField = value;
            }
        }

        public TextType Description
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

        public CodeType MeasurementMethodCode
        {
            get
            {
                return this.measurementMethodCodeField;
            }
            set
            {
                this.measurementMethodCodeField = value;
            }
        }

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

        public MeasureType CapacityValueMeasure
        {
            get
            {
                return this.capacityValueMeasureField;
            }
            set
            {
                this.capacityValueMeasureField = value;
            }
        }

        public CodeType CertificationCode
        {
            get
            {
                return this.certificationCodeField;
            }
            set
            {
                this.certificationCodeField = value;
            }
        }

        public TextType Licence
        {
            get
            {
                return this.licenceField;
            }
            set
            {
                this.licenceField = value;
            }
        }

        public DateTimeType ConstructionDateTime
        {
            get
            {
                return this.constructionDateTimeField;
            }
            set
            {
                this.constructionDateTimeField = value;
            }
        }

        public DateTimeType LatestRenovationDateTime
        {
            get
            {
                return this.latestRenovationDateTimeField;
            }
            set
            {
                this.latestRenovationDateTimeField = value;
            }
        }

        [XmlElement("DescriptionCode")]
        public CodeType[] DescriptionCode
        {
            get
            {
                return this.descriptionCodeField;
            }
            set
            {
                this.descriptionCodeField = value;
            }
        }

        [XmlElement("SpecifiedAnimalHoldingEvent")]
        public AnimalHoldingEventType[] SpecifiedAnimalHoldingEvent
        {
            get
            {
                return this.specifiedAnimalHoldingEventField;
            }
            set
            {
                this.specifiedAnimalHoldingEventField = value;
            }
        }

        [XmlElement("SpecifiedTTProductProcessEvent")]
        public TTProductProcessEventType[] SpecifiedTTProductProcessEvent
        {
            get
            {
                return this.specifiedTTProductProcessEventField;
            }
            set
            {
                this.specifiedTTProductProcessEventField = value;
            }
        }
    }
}