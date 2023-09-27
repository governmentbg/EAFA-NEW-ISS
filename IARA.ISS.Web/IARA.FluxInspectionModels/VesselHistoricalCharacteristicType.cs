namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselHistoricalCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselHistoricalCharacteristicType
    {

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private TextType valueField;

        private IndicatorType valueIndicatorField;

        private CodeType valueCodeField;

        private DateTimeType valueDateTimeField;

        private MeasureType valueMeasureField;

        private QuantityType valueQuantityField;

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

        public TextType Value
        {
            get => this.valueField;
            set => this.valueField = value;
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

        public DateTimeType ValueDateTime
        {
            get => this.valueDateTimeField;
            set => this.valueDateTimeField = value;
        }

        public MeasureType ValueMeasure
        {
            get => this.valueMeasureField;
            set => this.valueMeasureField = value;
        }

        public QuantityType ValueQuantity
        {
            get => this.valueQuantityField;
            set => this.valueQuantityField = value;
        }
    }
}