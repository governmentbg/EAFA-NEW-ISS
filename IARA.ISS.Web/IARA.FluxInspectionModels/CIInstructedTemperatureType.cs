namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIInstructedTemperature", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIInstructedTemperatureType
    {

        private MeasureType maximumValueMeasureField;

        private MeasureType minimumValueMeasureField;

        private CodeType controlCodeField;

        public MeasureType MaximumValueMeasure
        {
            get
            {
                return this.maximumValueMeasureField;
            }
            set
            {
                this.maximumValueMeasureField = value;
            }
        }

        public MeasureType MinimumValueMeasure
        {
            get
            {
                return this.minimumValueMeasureField;
            }
            set
            {
                this.minimumValueMeasureField = value;
            }
        }

        public CodeType ControlCode
        {
            get
            {
                return this.controlCodeField;
            }
            set
            {
                this.controlCodeField = value;
            }
        }
    }
}