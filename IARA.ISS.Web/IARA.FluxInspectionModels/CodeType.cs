namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class CodeType
    {

        private string listIDField;

        private string listAgencyIDField;

        private string listAgencyNameField;

        private string listVersionIDField;

        private string nameField;

        private string listNameField;

        private string languageIDField;

        private string listURIField;

        private string listSchemeURIField;

        private string valueField;

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

        [XmlAttribute]
        public string listAgencyName
        {
            get
            {
                return this.listAgencyNameField;
            }
            set
            {
                this.listAgencyNameField = value;
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

        [XmlAttribute]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlAttribute]
        public string listName
        {
            get
            {
                return this.listNameField;
            }
            set
            {
                this.listNameField = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string languageID
        {
            get
            {
                return this.languageIDField;
            }
            set
            {
                this.languageIDField = value;
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

        [XmlAttribute(DataType = "anyURI")]
        public string listSchemeURI
        {
            get
            {
                return this.listSchemeURIField;
            }
            set
            {
                this.listSchemeURIField = value;
            }
        }

        [XmlText(DataType = "token")]
        public string Value
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