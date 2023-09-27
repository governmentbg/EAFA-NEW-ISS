namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DataSetVersion", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DataSetVersionType
    {

        private IDType idField;

        private TextType nameField;

        private DateTimeType validityStartDateTimeField;

        private DateTimeType validityEndDateTimeField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public DateTimeType ValidityStartDateTime
        {
            get => this.validityStartDateTimeField;
            set => this.validityStartDateTimeField = value;
        }

        public DateTimeType ValidityEndDateTime
        {
            get => this.validityEndDateTimeField;
            set => this.validityEndDateTimeField = value;
        }
    }
}