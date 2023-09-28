namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselStorageCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselStorageCharacteristicType
    {

        private CodeType[] typeCodeField;

        private CodeType methodTypeCodeField;

        private MeasureType[] capacityValueMeasureField;

        private MeasureType[] temperatureValueMeasureField;

        private QuantityType unitValueQuantityField;

        private IDType idField;

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

        public CodeType MethodTypeCode
        {
            get
            {
                return this.methodTypeCodeField;
            }
            set
            {
                this.methodTypeCodeField = value;
            }
        }

        [XmlElement("CapacityValueMeasure")]
        public MeasureType[] CapacityValueMeasure
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

        [XmlElement("TemperatureValueMeasure")]
        public MeasureType[] TemperatureValueMeasure
        {
            get
            {
                return this.temperatureValueMeasureField;
            }
            set
            {
                this.temperatureValueMeasureField = value;
            }
        }

        public QuantityType UnitValueQuantity
        {
            get
            {
                return this.unitValueQuantityField;
            }
            set
            {
                this.unitValueQuantityField = value;
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
    }
}