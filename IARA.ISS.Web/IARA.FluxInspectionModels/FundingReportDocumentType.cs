namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FundingReportDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FundingReportDocumentType
    {

        private DateTimeType creationDateTimeField;

        private TextType descriptionField;

        private TextType amountFactorDescriptionField;

        private DateTimeType lastReportedSubmissionDateTimeField;

        private BasePeriodType effectiveBasePeriodField;

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TextType AmountFactorDescription
        {
            get => this.amountFactorDescriptionField;
            set => this.amountFactorDescriptionField = value;
        }

        public DateTimeType LastReportedSubmissionDateTime
        {
            get => this.lastReportedSubmissionDateTimeField;
            set => this.lastReportedSubmissionDateTimeField = value;
        }

        public BasePeriodType EffectiveBasePeriod
        {
            get => this.effectiveBasePeriodField;
            set => this.effectiveBasePeriodField = value;
        }
    }
}