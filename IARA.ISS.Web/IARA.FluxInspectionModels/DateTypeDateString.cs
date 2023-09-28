namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class DateTypeDateString
    {

        private string formatField;

        private string valueField;

        [XmlAttribute]
        public string format
        {
            get
            {
                return this.formatField;
            }
            set
            {
                this.formatField = value;
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