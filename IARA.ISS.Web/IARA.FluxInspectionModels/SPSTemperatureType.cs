namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSTemperature", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSTemperatureType
    {

        private MeasureType valueMeasureField;

        private MeasureType minimumValueMeasureField;

        private MeasureType maximumValueMeasureField;

        private TemperatureTypeCodeType typeCodeField;

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

        public TemperatureTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }
    }
}