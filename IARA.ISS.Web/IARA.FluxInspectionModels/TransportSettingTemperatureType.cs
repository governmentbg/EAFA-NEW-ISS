namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TransportSettingTemperature", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TransportSettingTemperatureType
    {

        private TemperatureUnitMeasureType valueMeasureField;

        private TemperatureUnitMeasureType minimumValueMeasureField;

        private TemperatureUnitMeasureType maximumValueMeasureField;

        private TemperatureTypeCodeType typeCodeField;

        private TemperatureSettingInstructionsType informationTemperatureSettingInstructionsField;

        public TemperatureUnitMeasureType ValueMeasure
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

        public TemperatureUnitMeasureType MinimumValueMeasure
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

        public TemperatureUnitMeasureType MaximumValueMeasure
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

        public TemperatureTypeCodeType TypeCode
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

        public TemperatureSettingInstructionsType InformationTemperatureSettingInstructions
        {
            get
            {
                return this.informationTemperatureSettingInstructionsField;
            }
            set
            {
                this.informationTemperatureSettingInstructionsField = value;
            }
        }
    }
}