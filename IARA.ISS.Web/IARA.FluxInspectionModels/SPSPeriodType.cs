namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSPeriod", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSPeriodType
    {

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

        private MeasureType durationMeasureField;

        public DateTimeType StartDateTime
        {
            get => this.startDateTimeField;
            set => this.startDateTimeField = value;
        }

        public DateTimeType EndDateTime
        {
            get => this.endDateTimeField;
            set => this.endDateTimeField = value;
        }

        public MeasureType DurationMeasure
        {
            get => this.durationMeasureField;
            set => this.durationMeasureField = value;
        }
    }
}