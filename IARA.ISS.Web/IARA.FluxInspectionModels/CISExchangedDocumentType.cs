namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISExchangedDocumentType
    {

        private IDType globalIDField;

        private IDType previousRevisionIDField;

        private IDType revisionIDField;

        private IDType versionIDField;

        private IDType idField;

        private IDType[] languageIDField;

        private MessageFunctionCodeType amendmentPurposeCodeField;

        private MessageFunctionCodeType purposeCodeField;

        private ResponseTypeCodeType[] requestedResponseTypeCodeField;

        private DocumentStatusCodeType statusCodeField;

        private SchedulingDocumentCodeType typeCodeField;

        private DateTimeType cancellationDateTimeField;

        private DateTimeType issueDateTimeField;

        private DateTimeType revisionDateTimeField;

        private TextType purposeField;

        private IndicatorType copyIndicatorField;

        private NumericType lineCountNumericField;

        private TextType nameField;

        private DateTimeType acceptanceDateTimeField;

        private CodeType categoryCodeField;

        private TextType[] informationField;

        private QuantityType lineItemQuantityField;

        private DateTimeType[] rejectionResponseDateTimeField;

        private TextType[] remarksField;

        private DateTimeType[] responseDateTimeField;

        private CodeType[] responseReasonCodeField;

        private DateTimeType submissionDateTimeField;

        private CodeType urgencyCodeField;

        private CITradePartyType issuerCITradePartyField;

        private SpecifiedBinaryFileType[] attachedSpecifiedBinaryFileField;

        private CINoteType[] includedCINoteField;

        private CISpecifiedPeriodType effectiveCISpecifiedPeriodField;

        private CIDocumentAuthenticationType buyerSignatoryCIDocumentAuthenticationField;

        private CIDocumentAuthenticationType approverSignatoryCIDocumentAuthenticationField;

        public IDType GlobalID
        {
            get => this.globalIDField;
            set => this.globalIDField = value;
        }

        public IDType PreviousRevisionID
        {
            get => this.previousRevisionIDField;
            set => this.previousRevisionIDField = value;
        }

        public IDType RevisionID
        {
            get => this.revisionIDField;
            set => this.revisionIDField = value;
        }

        public IDType VersionID
        {
            get => this.versionIDField;
            set => this.versionIDField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("LanguageID")]
        public IDType[] LanguageID
        {
            get => this.languageIDField;
            set => this.languageIDField = value;
        }

        public MessageFunctionCodeType AmendmentPurposeCode
        {
            get => this.amendmentPurposeCodeField;
            set => this.amendmentPurposeCodeField = value;
        }

        public MessageFunctionCodeType PurposeCode
        {
            get => this.purposeCodeField;
            set => this.purposeCodeField = value;
        }

        [XmlElement("RequestedResponseTypeCode")]
        public ResponseTypeCodeType[] RequestedResponseTypeCode
        {
            get => this.requestedResponseTypeCodeField;
            set => this.requestedResponseTypeCodeField = value;
        }

        public DocumentStatusCodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public SchedulingDocumentCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DateTimeType CancellationDateTime
        {
            get => this.cancellationDateTimeField;
            set => this.cancellationDateTimeField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public DateTimeType RevisionDateTime
        {
            get => this.revisionDateTimeField;
            set => this.revisionDateTimeField = value;
        }

        public TextType Purpose
        {
            get => this.purposeField;
            set => this.purposeField = value;
        }

        public IndicatorType CopyIndicator
        {
            get => this.copyIndicatorField;
            set => this.copyIndicatorField = value;
        }

        public NumericType LineCountNumeric
        {
            get => this.lineCountNumericField;
            set => this.lineCountNumericField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public DateTimeType AcceptanceDateTime
        {
            get => this.acceptanceDateTimeField;
            set => this.acceptanceDateTimeField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        [XmlElement("Information")]
        public TextType[] Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public QuantityType LineItemQuantity
        {
            get => this.lineItemQuantityField;
            set => this.lineItemQuantityField = value;
        }

        [XmlElement("RejectionResponseDateTime")]
        public DateTimeType[] RejectionResponseDateTime
        {
            get => this.rejectionResponseDateTimeField;
            set => this.rejectionResponseDateTimeField = value;
        }

        [XmlElement("Remarks")]
        public TextType[] Remarks
        {
            get => this.remarksField;
            set => this.remarksField = value;
        }

        [XmlElement("ResponseDateTime")]
        public DateTimeType[] ResponseDateTime
        {
            get => this.responseDateTimeField;
            set => this.responseDateTimeField = value;
        }

        [XmlElement("ResponseReasonCode")]
        public CodeType[] ResponseReasonCode
        {
            get => this.responseReasonCodeField;
            set => this.responseReasonCodeField = value;
        }

        public DateTimeType SubmissionDateTime
        {
            get => this.submissionDateTimeField;
            set => this.submissionDateTimeField = value;
        }

        public CodeType UrgencyCode
        {
            get => this.urgencyCodeField;
            set => this.urgencyCodeField = value;
        }

        public CITradePartyType IssuerCITradeParty
        {
            get => this.issuerCITradePartyField;
            set => this.issuerCITradePartyField = value;
        }

        [XmlElement("AttachedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] AttachedSpecifiedBinaryFile
        {
            get => this.attachedSpecifiedBinaryFileField;
            set => this.attachedSpecifiedBinaryFileField = value;
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get => this.includedCINoteField;
            set => this.includedCINoteField = value;
        }

        public CISpecifiedPeriodType EffectiveCISpecifiedPeriod
        {
            get => this.effectiveCISpecifiedPeriodField;
            set => this.effectiveCISpecifiedPeriodField = value;
        }

        public CIDocumentAuthenticationType BuyerSignatoryCIDocumentAuthentication
        {
            get => this.buyerSignatoryCIDocumentAuthenticationField;
            set => this.buyerSignatoryCIDocumentAuthenticationField = value;
        }

        public CIDocumentAuthenticationType ApproverSignatoryCIDocumentAuthentication
        {
            get => this.approverSignatoryCIDocumentAuthenticationField;
            set => this.approverSignatoryCIDocumentAuthenticationField = value;
        }
    }
}