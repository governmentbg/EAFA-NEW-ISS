namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CostScheduleReportDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CostScheduleReportDocumentType
    {

        private DateTimeType creationDateTimeField;

        private DateTimeType submissionDateTimeField;

        private TextType descriptionField;

        private SecurityClassificationTypeCodeType securityTypeCodeField;

        private ScheduleTypeCodeType scheduleTypeCodeField;

        private BasePeriodType effectiveBasePeriodField;

        private ProjectPartyType ownerProjectPartyField;

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        public DateTimeType SubmissionDateTime
        {
            get => this.submissionDateTimeField;
            set => this.submissionDateTimeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public SecurityClassificationTypeCodeType SecurityTypeCode
        {
            get => this.securityTypeCodeField;
            set => this.securityTypeCodeField = value;
        }

        public ScheduleTypeCodeType ScheduleTypeCode
        {
            get => this.scheduleTypeCodeField;
            set => this.scheduleTypeCodeField = value;
        }

        public BasePeriodType EffectiveBasePeriod
        {
            get => this.effectiveBasePeriodField;
            set => this.effectiveBasePeriodField = value;
        }

        public ProjectPartyType OwnerProjectParty
        {
            get => this.ownerProjectPartyField;
            set => this.ownerProjectPartyField = value;
        }
    }
}