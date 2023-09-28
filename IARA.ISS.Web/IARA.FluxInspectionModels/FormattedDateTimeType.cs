namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class FormattedDateTimeType
    {

        private FormattedDateTimeTypeDateTimeString dateTimeStringField;

        public FormattedDateTimeTypeDateTimeString DateTimeString
        {
            get
            {
                return this.dateTimeStringField;
            }
            set
            {
                this.dateTimeStringField = value;
            }
        }
    }
}