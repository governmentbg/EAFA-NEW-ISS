namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GearCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GearCharacteristicType
    {

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private MeasureType valueMeasureField;

        private DateTimeType valueDateTimeField;

        private IndicatorType valueIndicatorField;

        private CodeType valueCodeField;

        private TextType valueField;

        private QuantityType valueQuantityField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        private FLUXBinaryFileType valueFLUXBinaryFileField;

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

        public DateTimeType ValueDateTime
        {
            get
            {
                return this.valueDateTimeField;
            }
            set
            {
                this.valueDateTimeField = value;
            }
        }

        public IndicatorType ValueIndicator
        {
            get
            {
                return this.valueIndicatorField;
            }
            set
            {
                this.valueIndicatorField = value;
            }
        }

        public CodeType ValueCode
        {
            get
            {
                return this.valueCodeField;
            }
            set
            {
                this.valueCodeField = value;
            }
        }

        public TextType Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        public QuantityType ValueQuantity
        {
            get
            {
                return this.valueQuantityField;
            }
            set
            {
                this.valueQuantityField = value;
            }
        }

        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation
        {
            get
            {
                return this.specifiedFLUXLocationField;
            }
            set
            {
                this.specifiedFLUXLocationField = value;
            }
        }

        public FLUXBinaryFileType ValueFLUXBinaryFile
        {
            get
            {
                return this.valueFLUXBinaryFileField;
            }
            set
            {
                this.valueFLUXBinaryFileField = value;
            }
        }
    }
}