namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIIHExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIIHExchangedDocumentType
    {

        private IDType idField;

        private TextType[] nameField;

        private InvoiceDocumentCodeType typeCodeField;

        private DateTimeType issueDateTimeField;

        private IndicatorType copyIndicatorField;

        private TextType purposeField;

        private IDType[] languageIDField;

        private MessageFunctionCodeType purposeCodeField;

        private DateTimeType revisionDateTimeField;

        private IDType versionIDField;

        private IDType globalIDField;

        private IDType revisionIDField;

        private IDType previousRevisionIDField;

        private CodeType categoryCodeField;

        private IndicatorType controlRequirementIndicatorField;

        private CodeType responseReasonCodeField;

        private CodeType subtypeCodeField;

        private CodeType[] requestedResponseTypeCodeField;

        private CINoteType[] includedCINoteField;

        private CITradePartyType issuerCITradePartyField;

        private CIDocumentAuthenticationType signatoryCIDocumentAuthenticationField;

        private CIReferencedDocumentType[] referenceCIReferencedDocumentField;

        private SpecifiedBinaryFileType[] attachedSpecifiedBinaryFileField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public InvoiceDocumentCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public IndicatorType CopyIndicator
        {
            get => this.copyIndicatorField;
            set => this.copyIndicatorField = value;
        }

        public TextType Purpose
        {
            get => this.purposeField;
            set => this.purposeField = value;
        }

        [XmlElement("LanguageID")]
        public IDType[] LanguageID
        {
            get => this.languageIDField;
            set => this.languageIDField = value;
        }

        public MessageFunctionCodeType PurposeCode
        {
            get => this.purposeCodeField;
            set => this.purposeCodeField = value;
        }

        public DateTimeType RevisionDateTime
        {
            get => this.revisionDateTimeField;
            set => this.revisionDateTimeField = value;
        }

        public IDType VersionID
        {
            get => this.versionIDField;
            set => this.versionIDField = value;
        }

        public IDType GlobalID
        {
            get => this.globalIDField;
            set => this.globalIDField = value;
        }

        public IDType RevisionID
        {
            get => this.revisionIDField;
            set => this.revisionIDField = value;
        }

        public IDType PreviousRevisionID
        {
            get => this.previousRevisionIDField;
            set => this.previousRevisionIDField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public IndicatorType ControlRequirementIndicator
        {
            get => this.controlRequirementIndicatorField;
            set => this.controlRequirementIndicatorField = value;
        }

        public CodeType ResponseReasonCode
        {
            get => this.responseReasonCodeField;
            set => this.responseReasonCodeField = value;
        }

        public CodeType SubtypeCode
        {
            get => this.subtypeCodeField;
            set => this.subtypeCodeField = value;
        }

        [XmlElement("RequestedResponseTypeCode")]
        public CodeType[] RequestedResponseTypeCode
        {
            get => this.requestedResponseTypeCodeField;
            set => this.requestedResponseTypeCodeField = value;
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get => this.includedCINoteField;
            set => this.includedCINoteField = value;
        }

        public CITradePartyType IssuerCITradeParty
        {
            get => this.issuerCITradePartyField;
            set => this.issuerCITradePartyField = value;
        }

        public CIDocumentAuthenticationType SignatoryCIDocumentAuthentication
        {
            get => this.signatoryCIDocumentAuthenticationField;
            set => this.signatoryCIDocumentAuthenticationField = value;
        }

        [XmlElement("ReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] ReferenceCIReferencedDocument
        {
            get => this.referenceCIReferencedDocumentField;
            set => this.referenceCIReferencedDocumentField = value;
        }

        [XmlElement("AttachedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] AttachedSpecifiedBinaryFile
        {
            get => this.attachedSpecifiedBinaryFileField;
            set => this.attachedSpecifiedBinaryFileField = value;
        }
    }
}