namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class DateOnlyFormattedDateTimeType
    {

        private DateOnlyFormattedDateTimeTypeDateTimeString dateTimeStringField;

        public DateOnlyFormattedDateTimeTypeDateTimeString DateTimeString
        {
            get => this.dateTimeStringField;
            set => this.dateTimeStringField = value;
        }
    }
}