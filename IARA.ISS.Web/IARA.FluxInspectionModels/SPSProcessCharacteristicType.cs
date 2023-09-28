namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSProcessCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSProcessCharacteristicType
    {

        private MeasuredAttributeCodeType typeCodeField;

        private TextType[] descriptionField;

        private MeasureType valueMeasureField;

        private MeasureType minimumValueMeasureField;

        private MeasureType maximumValueMeasureField;

        public MeasuredAttributeCodeType TypeCode
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

        public MeasureType MinimumValueMeasure
        {
            get => this.minimumValueMeasureField;
            set => this.minimumValueMeasureField = value;
        }

        public MeasureType MaximumValueMeasure
        {
            get => this.maximumValueMeasureField;
            set => this.maximumValueMeasureField = value;
        }
    }
}