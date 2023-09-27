namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class TextType
    {

        private string languageIDField;

        private string languageLocaleIDField;

        private string valueField;

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

        [XmlAttribute(DataType = "token")]
        public string languageLocaleID
        {
            get
            {
                return this.languageLocaleIDField;
            }
            set
            {
                this.languageLocaleIDField = value;
            }
        }

        [XmlText]
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