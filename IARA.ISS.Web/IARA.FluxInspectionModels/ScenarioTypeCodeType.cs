namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class ScenarioTypeCodeType
    {

        private string listIDField;

        private string listAgencyIDField;

        private string listVersionIDField;

        private string listURIField;

        private ScenarioTypeContentType valueField;

        public ScenarioTypeCodeType()
        {
            this.listIDField = "ScenarioTypeE018";
            this.listAgencyIDField = "210";
            this.listVersionIDField = "D11A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string listAgencyID
        {
            get
            {
                return this.listAgencyIDField;
            }
            set
            {
                this.listAgencyIDField = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string listVersionID
        {
            get
            {
                return this.listVersionIDField;
            }
            set
            {
                this.listVersionIDField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string listURI
        {
            get
            {
                return this.listURIField;
            }
            set
            {
                this.listURIField = value;
            }
        }

        [XmlText]
        public ScenarioTypeContentType Value
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