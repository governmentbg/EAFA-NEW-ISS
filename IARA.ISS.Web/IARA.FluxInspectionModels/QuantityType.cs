namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class QuantityType
    {

        private string unitCodeField;

        private string unitCodeListIDField;

        private string unitCodeListAgencyIDField;

        private string unitCodeListAgencyNameField;

        private decimal valueField;

        [XmlAttribute(DataType = "token")]
        public string unitCode
        {
            get => this.unitCodeField;
            set => this.unitCodeField = value;
        }

        [XmlAttribute(DataType = "token")]
        public string unitCodeListID
        {
            get => this.unitCodeListIDField;
            set => this.unitCodeListIDField = value;
        }

        [XmlAttribute(DataType = "token")]
        public string unitCodeListAgencyID
        {
            get => this.unitCodeListAgencyIDField;
            set => this.unitCodeListAgencyIDField = value;
        }

        [XmlAttribute]
        public string unitCodeListAgencyName
        {
            get => this.unitCodeListAgencyNameField;
            set => this.unitCodeListAgencyNameField = value;
        }

        [XmlText]
        public decimal Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}