namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class MeasureType
    {

        private string unitCodeField;

        private string unitCodeListVersionIDField;

        private decimal valueField;

        [XmlAttribute(DataType = "token")]
        public string unitCode
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