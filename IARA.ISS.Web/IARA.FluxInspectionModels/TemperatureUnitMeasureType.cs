namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class TemperatureUnitMeasureType
    {

        private MeasurementUnitCommonCodeTemperatureContentType unitCodeField;

        private bool unitCodeFieldSpecified;

        private string unitCodeListVersionIDField;

        private decimal valueField;

        [XmlAttribute]
        public MeasurementUnitCommonCodeTemperatureContentType unitCode
        {
            get
            {
                return this.unitCodeField;
            }
            set
            {
                this.unitCodeField = value;
            }
        }

        [XmlIgnore]
        public bool unitCodeSpecified
        {
            get
            {
                return this.unitCodeFieldSpecified;
            }
            set
            {
                this.unitCodeFieldSpecified = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string unitCodeListVersionID
        {
            get
            {
                return this.unitCodeListVersionIDField;
            }
            set
            {
                this.unitCodeListVersionIDField = value;
            }
        }

        [XmlText]
        public decimal Value
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
    }
}