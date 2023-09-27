namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAArchiveDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAArchiveDocumentType
    {

        private IndicatorType multipleTypeIndicatorField;

        private IDType idField;

        private AccountingDocumentCodeType[] typeCodeField;

        private TextType nameField;

        private TextType purposeField;

        private TextType[] descriptionField;

        private DateTimeType issueDateTimeField;

        private DateTimeType submissionDateTimeField;

        private DateTimeType receiptDateTimeField;

        private IndicatorType controlRequirementIndicatorField;

        private DateTimeType creationDateTimeField;

        private DocumentStatusCodeType[] statusCodeField;

        private IndicatorType copyIndicatorField;

        private DateTimeType responseDateTimeField;

        private IDType itemIdentificationIDField;

        private TextType[] remarksField;

        private IDType[] languageIDField;

        private CurrencyCodeType[] currencyCodeField;

        private NumericType[] lineCountNumericField;

        private IDType[] lineIDField;

        private IndicatorType multipleReferencesIndicatorField;

        private TextType[] revisionField;

        private TextType authorizationField;

        private NumericType checksumNumericField;

        private QuantityType[] itemQuantityField;

        private DateTimeType acceptanceDateTimeField;

        private QuantityType originalRequiredQuantityField;

        private QuantityType copyRequiredQuantityField;

        private QuantityType originalIssuedQuantityField;

        private QuantityType copyIssuedQuantityField;

        private TextType informationField;

        private IndicatorType authenticatedOriginalIndicatorField;

        private TextType[] dispositionField;

        private IndicatorType electronicPresentationIndicatorField;

        private IDType[] pageIDField;

        private QuantityType totalPageQuantityField;

        private DateTimeType revisionDateTimeField;

        private TextType[] rejectionReasonField;

        private TextType[] cancellationReasonField;

        private DateTimeType cancellationDateTimeField;

        private IDType[] sequenceIDField;

        private TextType[] sectionNameField;

        private IDType externalIDField;

        private DateTimeType firstVersionIssueDateTimeField;

        private IndicatorType examinedIndicatorField;

        private IndicatorType verifiedIndicatorField;

        private IndicatorType handwrittenIndicatorField;

        private IndicatorType printedIndicatorField;

        private IndicatorType signedIndicatorField;

        private DateTimeType signedDateTimeField;

        private DateTimeType requestedDateTimeField;

        private TextType[] signatureLocationField;

        private TextType[] statusField;

        private AmountType[] includedAmountField;

        private QuantityType[] includedQuantityField;

        private DateTimeType rejectionDateTimeField;

        private AAAArchiveArchiveParameterType specifiedAAAArchiveArchiveParameterField;

        private AAAArchivePartyType ownerAAAArchivePartyField;

        private AAAArchivePartyType[] agentAAAArchivePartyField;

        private AAAArchivePartyType senderAAAArchivePartyField;

        private AAAArchiveNoteType[] includedAAAArchiveNoteField;

        private AAAArchiveBinaryFileType[] attachedAAAArchiveBinaryFileField;

        private AAAPeriodType effectiveAAAPeriodField;

        private AAAPeriodType acceptableAAAPeriodField;

        private AAAArchiveLocationType[] lodgementAAAArchiveLocationField;

        private AAAArchiveLocationType[] issueAAAArchiveLocationField;

        private AAAArchiveClauseType[] contractualAAAArchiveClauseField;

        private AAAArchiveSoftwareType productionAAAArchiveSoftwareField;

        private AAAArchiveDocumentType[] referenceAAAArchiveDocumentField;

        private AAAArchiveAuthenticationType[] signatoryAAAArchiveAuthenticationField;

        public IndicatorType MultipleTypeIndicator
        {
            get => this.multipleTypeIndicatorField;
            set => this.multipleTypeIndicatorField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("TypeCode")]
        public AccountingDocumentCodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType Purpose
        {
            get => this.purposeField;
            set => this.purposeField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public DateTimeType SubmissionDateTime
        {
            get => this.submissionDateTimeField;
            set => this.submissionDateTimeField = value;
        }

        public DateTimeType ReceiptDateTime
        {
            get => this.receiptDateTimeField;
            set => this.receiptDateTimeField = value;
        }

        public IndicatorType ControlRequirementIndicator
        {
            get => this.controlRequirementIndicatorField;
            set => this.controlRequirementIndicatorField = value;
        }

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        [XmlElement("StatusCode")]
        public DocumentStatusCodeType[] StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public IndicatorType CopyIndicator
        {
            get => this.copyIndicatorField;
            set => this.copyIndicatorField = value;
        }

        public DateTimeType ResponseDateTime
        {
            get => this.responseDateTimeField;
            set => this.responseDateTimeField = value;
        }

        public IDType ItemIdentificationID
        {
            get => this.itemIdentificationIDField;
            set => this.itemIdentificationIDField = value;
        }

        [XmlElement("Remarks")]
        public TextType[] Remarks
        {
            get => this.remarksField;
            set => this.remarksField = value;
        }

        [XmlElement("LanguageID")]
        public IDType[] LanguageID
        {
            get => this.languageIDField;
            set => this.languageIDField = value;
        }

        [XmlElement("CurrencyCode")]
        public CurrencyCodeType[] CurrencyCode
        {
            get => this.currencyCodeField;
            set => this.currencyCodeField = value;
        }

        [XmlElement("LineCountNumeric")]
        public NumericType[] LineCountNumeric
        {
            get => this.lineCountNumericField;
            set => this.lineCountNumericField = value;
        }

        [XmlElement("LineID")]
        public IDType[] LineID
        {
            get => this.lineIDField;
            set => this.lineIDField = value;
        }

        public IndicatorType MultipleReferencesIndicator
        {
            get => this.multipleReferencesIndicatorField;
            set => this.multipleReferencesIndicatorField = value;
        }

        [XmlElement("Revision")]
        public TextType[] Revision
        {
            get => this.revisionField;
            set => this.revisionField = value;
        }

        public TextType Authorization
        {
            get => this.authorizationField;
            set => this.authorizationField = value;
        }

        public NumericType ChecksumNumeric
        {
            get => this.checksumNumericField;
            set => this.checksumNumericField = value;
        }

        [XmlElement("ItemQuantity")]
        public QuantityType[] ItemQuantity
        {
            get => this.itemQuantityField;
            set => this.itemQuantityField = value;
        }

        public DateTimeType AcceptanceDateTime
        {
            get => this.acceptanceDateTimeField;
            set => this.acceptanceDateTimeField = value;
        }

        public QuantityType OriginalRequiredQuantity
        {
            get => this.originalRequiredQuantityField;
            set => this.originalRequiredQuantityField = value;
        }

        public QuantityType CopyRequiredQuantity
        {
            get => this.copyRequiredQuantityField;
            set => this.copyRequiredQuantityField = value;
        }

        public QuantityType OriginalIssuedQuantity
        {
            get => this.originalIssuedQuantityField;
            set => this.originalIssuedQuantityField = value;
        }

        public QuantityType CopyIssuedQuantity
        {
            get => this.copyIssuedQuantityField;
            set => this.copyIssuedQuantityField = value;
        }

        public TextType Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public IndicatorType AuthenticatedOriginalIndicator
        {
            get => this.authenticatedOriginalIndicatorField;
            set => this.authenticatedOriginalIndicatorField = value;
        }

        [XmlElement("Disposition")]
        public TextType[] Disposition
        {
            get => this.dispositionField;
            set => this.dispositionField = value;
        }

        public IndicatorType ElectronicPresentationIndicator
        {
            get => this.electronicPresentationIndicatorField;
            set => this.electronicPresentationIndicatorField = value;
        }

        [XmlElement("PageID")]
        public IDType[] PageID
        {
            get => this.pageIDField;
            set => this.pageIDField = value;
        }

        public QuantityType TotalPageQuantity
        {
            get => this.totalPageQuantityField;
            set => this.totalPageQuantityField = value;
        }

        public DateTimeType RevisionDateTime
        {
            get => this.revisionDateTimeField;
            set => this.revisionDateTimeField = value;
        }

        [XmlElement("RejectionReason")]
        public TextType[] RejectionReason
        {
            get => this.rejectionReasonField;
            set => this.rejectionReasonField = value;
        }

        [XmlElement("CancellationReason")]
        public TextType[] CancellationReason
        {
            get => this.cancellationReasonField;
            set => this.cancellationReasonField = value;
        }

        public DateTimeType CancellationDateTime
        {
            get => this.cancellationDateTimeField;
            set => this.cancellationDateTimeField = value;
        }

        [XmlElement("SequenceID")]
        public IDType[] SequenceID
        {
            get => this.sequenceIDField;
            set => this.sequenceIDField = value;
        }

        [XmlElement("SectionName")]
        public TextType[] SectionName
        {
            get => this.sectionNameField;
            set => this.sectionNameField = value;
        }

        public IDType ExternalID
        {
            get => this.externalIDField;
            set => this.externalIDField = value;
        }

        public DateTimeType FirstVersionIssueDateTime
        {
            get => this.firstVersionIssueDateTimeField;
            set => this.firstVersionIssueDateTimeField = value;
        }

        public IndicatorType ExaminedIndicator
        {
            get => this.examinedIndicatorField;
            set => this.examinedIndicatorField = value;
        }

        public IndicatorType VerifiedIndicator
        {
            get => this.verifiedIndicatorField;
            set => this.verifiedIndicatorField = value;
        }

        public IndicatorType HandwrittenIndicator
        {
            get => this.handwrittenIndicatorField;
            set => this.handwrittenIndicatorField = value;
        }

        public IndicatorType PrintedIndicator
        {
            get => this.printedIndicatorField;
            set => this.printedIndicatorField = value;
        }

        public IndicatorType SignedIndicator
        {
            get => this.signedIndicatorField;
            set => this.signedIndicatorField = value;
        }

        public DateTimeType SignedDateTime
        {
            get => this.signedDateTimeField;
            set => this.signedDateTimeField = value;
        }

        public DateTimeType RequestedDateTime
        {
            get => this.requestedDateTimeField;
            set => this.requestedDateTimeField = value;
        }

        [XmlElement("SignatureLocation")]
        public TextType[] SignatureLocation
        {
            get => this.signatureLocationField;
            set => this.signatureLocationField = value;
        }

        [XmlElement("Status")]
        public TextType[] Status
        {
            get => this.statusField;
            set => this.statusField = value;
        }

        [XmlElement("IncludedAmount")]
        public AmountType[] IncludedAmount
        {
            get => this.includedAmountField;
            set => this.includedAmountField = value;
        }

        [XmlElement("IncludedQuantity")]
        public QuantityType[] IncludedQuantity
        {
            get => this.includedQuantityField;
            set => this.includedQuantityField = value;
        }

        public DateTimeType RejectionDateTime
        {
            get => this.rejectionDateTimeField;
            set => this.rejectionDateTimeField = value;
        }

        public AAAArchiveArchiveParameterType SpecifiedAAAArchiveArchiveParameter
        {
            get => this.specifiedAAAArchiveArchiveParameterField;
            set => this.specifiedAAAArchiveArchiveParameterField = value;
        }

        public AAAArchivePartyType OwnerAAAArchiveParty
        {
            get => this.ownerAAAArchivePartyField;
            set => this.ownerAAAArchivePartyField = value;
        }

        [XmlElement("AgentAAAArchiveParty")]
        public AAAArchivePartyType[] AgentAAAArchiveParty
        {
            get => this.agentAAAArchivePartyField;
            set => this.agentAAAArchivePartyField = value;
        }

        public AAAArchivePartyType SenderAAAArchiveParty
        {
            get => this.senderAAAArchivePartyField;
            set => this.senderAAAArchivePartyField = value;
        }

        [XmlElement("IncludedAAAArchiveNote")]
        public AAAArchiveNoteType[] IncludedAAAArchiveNote
        {
            get => this.includedAAAArchiveNoteField;
            set => this.includedAAAArchiveNoteField = value;
        }

        [XmlElement("AttachedAAAArchiveBinaryFile")]
        public AAAArchiveBinaryFileType[] AttachedAAAArchiveBinaryFile
        {
            get => this.attachedAAAArchiveBinaryFileField;
            set => this.attachedAAAArchiveBinaryFileField = value;
        }

        public AAAPeriodType EffectiveAAAPeriod
        {
            get => this.effectiveAAAPeriodField;
            set => this.effectiveAAAPeriodField = value;
        }

        public AAAPeriodType AcceptableAAAPeriod
        {
            get => this.acceptableAAAPeriodField;
            set => this.acceptableAAAPeriodField = value;
        }

        [XmlElement("LodgementAAAArchiveLocation")]
        public AAAArchiveLocationType[] LodgementAAAArchiveLocation
        {
            get => this.lodgementAAAArchiveLocationField;
            set => this.lodgementAAAArchiveLocationField = value;
        }

        [XmlElement("IssueAAAArchiveLocation")]
        public AAAArchiveLocationType[] IssueAAAArchiveLocation
        {
            get => this.issueAAAArchiveLocationField;
            set => this.issueAAAArchiveLocationField = value;
        }

        [XmlElement("ContractualAAAArchiveClause")]
        public AAAArchiveClauseType[] ContractualAAAArchiveClause
        {
            get => this.contractualAAAArchiveClauseField;
            set => this.contractualAAAArchiveClauseField = value;
        }

        public AAAArchiveSoftwareType ProductionAAAArchiveSoftware
        {
            get => this.productionAAAArchiveSoftwareField;
            set => this.productionAAAArchiveSoftwareField = value;
        }

        [XmlElement("ReferenceAAAArchiveDocument")]
        public AAAArchiveDocumentType[] ReferenceAAAArchiveDocument
        {
            get => this.referenceAAAArchiveDocumentField;
            set => this.referenceAAAArchiveDocumentField = value;
        }

        [XmlElement("SignatoryAAAArchiveAuthentication")]
        public AAAArchiveAuthenticationType[] SignatoryAAAArchiveAuthentication
        {
            get => this.signatoryAAAArchiveAuthenticationField;
            set => this.signatoryAAAArchiveAuthenticationField = value;
        }
    }
}