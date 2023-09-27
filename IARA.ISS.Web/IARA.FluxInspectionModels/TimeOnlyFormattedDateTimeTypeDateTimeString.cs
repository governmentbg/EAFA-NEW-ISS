namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class TimeOnlyFormattedDateTimeTypeDateTimeString
    {

        private TimeOnlyFormatCodeContentType formatField;

        private bool formatFieldSpecified;

        private string valueField;

        [XmlAttribute]
        public TimeOnlyFormatCodeContentType format
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

        [XmlIgnore]
        public bool formatSpecified
        {
            get
            {
                return this.formatFieldSpecified;
            }
            set
            {
                this.formatFieldSpecified = value;
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