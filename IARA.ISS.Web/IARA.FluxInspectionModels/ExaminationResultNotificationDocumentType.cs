namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ExaminationResultNotificationDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ExaminationResultNotificationDocumentType
    {

        private IDType idField;

        private TextType nameField;

        private DateTimeType issueDateTimeField;

        private TenderingPeriodType effectiveTenderingPeriodField;

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

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public TenderingPeriodType EffectiveTenderingPeriod
        {
            get => this.effectiveTenderingPeriodField;
            set => this.effectiveTenderingPeriodField = value;
        }
    }
}