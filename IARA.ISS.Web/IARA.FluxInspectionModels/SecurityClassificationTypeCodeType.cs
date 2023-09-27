namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class SecurityClassificationTypeCodeType
    {

        private string listIDField;

        private string listAgencyIDField;

        private string listVersionIDField;

        private SecurityClassificationTypeContentType valueField;

        public SecurityClassificationTypeCodeType()
        {
            this.listIDField = "9677";
            this.listAgencyIDField = "6";
            this.listVersionIDField = "D10B";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get => this.listIDField;
            set => this.listIDField = value;
        }

        [XmlAttribute(DataType = "token")]
        public string listAgencyID
        {
            get => this.listAgencyIDField;
            set => this.listAgencyIDField = value;
        }

        [XmlAttribute(DataType = "token")]
        public string listVersionID
        {
            get => this.listVersionIDField;
            set => this.listVersionIDField = value;
        }

        [XmlText]
        public SecurityClassificationTypeContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}