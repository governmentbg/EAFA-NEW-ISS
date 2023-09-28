namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class PriorityDescriptionCodeType
    {

        private string listIDField;

        private PriorityDescriptionCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private string listURIField;

        private PriorityDescriptionCodeContentType valueField;

        public PriorityDescriptionCodeType()
        {
            this.listIDField = "4037_PriorityDescriptionCode";
            this.listAgencyIDField = PriorityDescriptionCodeListAgencyIDContentType.Item6;
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get => this.listIDField;
            set => this.listIDField = value;
        }

        [XmlAttribute]
        public PriorityDescriptionCodeListAgencyIDContentType listAgencyID
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

        [XmlAttribute(DataType = "anyURI")]
        public string listURI
        {
            get => this.listURIField;
            set => this.listURIField = value;
        }

        [XmlText]
        public PriorityDescriptionCodeContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}