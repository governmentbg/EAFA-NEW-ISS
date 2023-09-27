namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TransactionPeriod", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TransactionPeriodType
    {

        private TextType descriptionField;

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

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