namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWDelimitedPeriod", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWDelimitedPeriodType
    {

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

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
    }
}