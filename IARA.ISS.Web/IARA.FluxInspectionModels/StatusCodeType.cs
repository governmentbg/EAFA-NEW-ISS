namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class StatusCodeType
    {

        private string listIDField;

        private StatusCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private string nameField;

        private string listURIField;

        private StatusCodeContentType valueField;

        public StatusCodeType()
        {
            this.listIDField = "4405";
            this.listAgencyIDField = StatusCodeListAgencyIDContentType.Item6;
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get => this.listIDField;
            set => this.listIDField = value;
        }

        [XmlAttribute]
        public StatusCodeListAgencyIDContentType listAgencyID
        {
            get => this.listAgencyIDField;
            set => this.listAgencyIDField = value;
        }

        [XmlIgnore()]
        public bool listAgencyIDSpecified
        {
            get => this.listAgencyIDFieldSpecified;
            set => this.listAgencyIDFieldSpecified = value;
        }

        [XmlAttribute(DataType = "token")]
        public string listVersionID
        {
            get => this.listVersionIDField;
            set => this.listVersionIDField = value;
        }

        [XmlAttribute]
        public string name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlAttribute(DataType = "anyURI")]
        public string listURI
        {
            get => this.listURIField;
            set => this.listURIField = value;
        }

        [XmlText]
        public StatusCodeContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}