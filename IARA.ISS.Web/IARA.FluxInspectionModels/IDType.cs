namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class IDType
    {

        private string schemeIDField;

        private string schemeNameField;

        private string schemeAgencyIDField;

        private string schemeAgencyNameField;

        private string schemeVersionIDField;

        private string schemeDataURIField;

        private string schemeURIField;

        private string valueField;

        [XmlAttribute(DataType = "token")]
        public string schemeID
        {
            get
            {
                return this.schemeIDField;
            }
            set
            {
                this.schemeIDField = value;
            }
        }

        [XmlAttribute]
        public string schemeName
        {
            get
            {
                return this.schemeNameField;
            }
            set
            {
                this.schemeNameField = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string schemeAgencyID
        {
            get
            {
                return this.schemeAgencyIDField;
            }
            set
            {
                this.schemeAgencyIDField = value;
            }
        }

        [XmlAttribute]
        public string schemeAgencyName
        {
            get
            {
                return this.schemeAgencyNameField;
            }
            set
            {
                this.schemeAgencyNameField = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string schemeVersionID
        {
            get
            {
                return this.schemeVersionIDField;
            }
            set
            {
                this.schemeVersionIDField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string schemeDataURI
        {
            get
            {
                return this.schemeDataURIField;
            }
            set
            {
                this.schemeDataURIField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string schemeURI
        {
            get
            {
                return this.schemeURIField;
            }
            set
            {
                this.schemeURIField = value;
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