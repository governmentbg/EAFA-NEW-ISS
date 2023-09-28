namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class TimeOnlyFormattedDateTimeType
    {

        private TimeOnlyFormattedDateTimeTypeDateTimeString dateTimeStringField;

        public TimeOnlyFormattedDateTimeTypeDateTimeString DateTimeString
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