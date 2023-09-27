namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ISRItemCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ISRItemCharacteristicType
    {

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private MeasureType valueMeasureField;

        private DateTimeType valueDateTimeField;

        private IndicatorType valueIndicatorField;

        private CodeType valueCodeField;

        private TextType[] valueField;

        private QuantityType valueQuantityField;

        private NumericType valueNumericField;

        private CodeType checkInspectionCodeField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public MeasureType ValueMeasure
        {
            get => this.valueMeasureField;
            set => this.valueMeasureField = value;
        }

        public DateTimeType ValueDateTime
        {
            get => this.valueDateTimeField;
            set => this.valueDateTimeField = value;
        }

        public IndicatorType ValueIndicator
        {
            get => this.valueIndicatorField;
            set => this.valueIndicatorField = value;
        }

        public CodeType ValueCode
        {
            get => this.valueCodeField;
            set => this.valueCodeField = value;
        }

        [XmlElement("Value")]
        public TextType[] Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }

        public QuantityType ValueQuantity
        {
            get => this.valueQuantityField;
            set => this.valueQuantityField = value;
        }

        public NumericType ValueNumeric
        {
            get => this.valueNumericField;
            set => this.valueNumericField = value;
        }

        public CodeType CheckInspectionCode
        {
            get => this.checkInspectionCodeField;
            set => this.checkInspectionCodeField = value;
        }

        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation
        {
            get => this.specifiedFLUXLocationField;
            set => this.specifiedFLUXLocationField = value;
        }
    }
}