namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQExchangedDocumentType
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

        private QuotationDocumentCodeType typeCodeField;

        private DateTimeType cancellationDateTimeField;

        private DateTimeType issueDateTimeField;

        private DateTimeType revisionDateTimeField;

        private TextType purposeField;

        private IndicatorType copyIndicatorField;

        private NumericType lineCountNumericField;

        private TextType[] nameField;

        private CITradePartyType issuerCITradePartyField;

        private CISpecifiedPeriodType effectiveCISpecifiedPeriodField;

        private SpecifiedBinaryFileType[] attachedSpecifiedBinaryFileField;

        private CINoteType[] includedCINoteField;

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

        public QuotationDocumentCodeType TypeCode
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

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public CITradePartyType IssuerCITradeParty
        {
            get => this.issuerCITradePartyField;
            set => this.issuerCITradePartyField = value;
        }

        public CISpecifiedPeriodType EffectiveCISpecifiedPeriod
        {
            get => this.effectiveCISpecifiedPeriodField;
            set => this.effectiveCISpecifiedPeriodField = value;
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
    }
}