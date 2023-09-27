namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderTenderingProcess", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderTenderingProcessType
    {

        private DateTimeType invitationToTenderEventDateTimeField;

        private DateTimeType explanationRequestDeadlineEventDateTimeField;

        private DateTimeType openingTendersEventDateTimeField;

        private DateTimeType tenderExplanationEventDateTimeField;

        private TextType evaluationRulesDescriptionField;

        private TextType instructionsDescriptionField;

        private TextType invalidCriteriaDescriptionField;

        private IDType idField;

        private TextType nameField;

        private CodeType tenderingProcedureCodeField;

        private CodeType judgingCriteriaCodeField;

        private CodeType tenderInvitationRevisionCodeField;

        private IndicatorType entryFeeIndicatorField;

        private AmountType entryFeeAmountField;

        private PercentType thresholdPricePercentField;

        private IndicatorType referenceDocumentIndicatorField;

        private IndicatorType urgencyIndicatorField;

        private ReferenceDocumentType[] requiredReferenceDocumentField;

        private TenderingPeriodType acceptanceTenderingPeriodField;

        private TransactionPeriodType[] specifiedTransactionPeriodField;

        private TenderingLocationType explanationSiteTenderingLocationField;

        private TenderingLocationType informationAccessSiteTenderingLocationField;

        public DateTimeType InvitationToTenderEventDateTime
        {
            get => this.invitationToTenderEventDateTimeField;
            set => this.invitationToTenderEventDateTimeField = value;
        }

        public DateTimeType ExplanationRequestDeadlineEventDateTime
        {
            get => this.explanationRequestDeadlineEventDateTimeField;
            set => this.explanationRequestDeadlineEventDateTimeField = value;
        }

        public DateTimeType OpeningTendersEventDateTime
        {
            get => this.openingTendersEventDateTimeField;
            set => this.openingTendersEventDateTimeField = value;
        }

        public DateTimeType TenderExplanationEventDateTime
        {
            get => this.tenderExplanationEventDateTimeField;
            set => this.tenderExplanationEventDateTimeField = value;
        }

        public TextType EvaluationRulesDescription
        {
            get => this.evaluationRulesDescriptionField;
            set => this.evaluationRulesDescriptionField = value;
        }

        public TextType InstructionsDescription
        {
            get => this.instructionsDescriptionField;
            set => this.instructionsDescriptionField = value;
        }

        public TextType InvalidCriteriaDescription
        {
            get => this.invalidCriteriaDescriptionField;
            set => this.invalidCriteriaDescriptionField = value;
        }

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

        public CodeType TenderingProcedureCode
        {
            get => this.tenderingProcedureCodeField;
            set => this.tenderingProcedureCodeField = value;
        }

        public CodeType JudgingCriteriaCode
        {
            get => this.judgingCriteriaCodeField;
            set => this.judgingCriteriaCodeField = value;
        }

        public CodeType TenderInvitationRevisionCode
        {
            get => this.tenderInvitationRevisionCodeField;
            set => this.tenderInvitationRevisionCodeField = value;
        }

        public IndicatorType EntryFeeIndicator
        {
            get => this.entryFeeIndicatorField;
            set => this.entryFeeIndicatorField = value;
        }

        public AmountType EntryFeeAmount
        {
            get => this.entryFeeAmountField;
            set => this.entryFeeAmountField = value;
        }

        public PercentType ThresholdPricePercent
        {
            get => this.thresholdPricePercentField;
            set => this.thresholdPricePercentField = value;
        }

        public IndicatorType ReferenceDocumentIndicator
        {
            get => this.referenceDocumentIndicatorField;
            set => this.referenceDocumentIndicatorField = value;
        }

        public IndicatorType UrgencyIndicator
        {
            get => this.urgencyIndicatorField;
            set => this.urgencyIndicatorField = value;
        }

        [XmlElement("RequiredReferenceDocument")]
        public ReferenceDocumentType[] RequiredReferenceDocument
        {
            get => this.requiredReferenceDocumentField;
            set => this.requiredReferenceDocumentField = value;
        }

        public TenderingPeriodType AcceptanceTenderingPeriod
        {
            get => this.acceptanceTenderingPeriodField;
            set => this.acceptanceTenderingPeriodField = value;
        }

        [XmlElement("SpecifiedTransactionPeriod")]
        public TransactionPeriodType[] SpecifiedTransactionPeriod
        {
            get => this.specifiedTransactionPeriodField;
            set => this.specifiedTransactionPeriodField = value;
        }

        public TenderingLocationType ExplanationSiteTenderingLocation
        {
            get => this.explanationSiteTenderingLocationField;
            set => this.explanationSiteTenderingLocationField = value;
        }

        public TenderingLocationType InformationAccessSiteTenderingLocation
        {
            get => this.informationAccessSiteTenderingLocationField;
            set => this.informationAccessSiteTenderingLocationField = value;
        }
    }
}