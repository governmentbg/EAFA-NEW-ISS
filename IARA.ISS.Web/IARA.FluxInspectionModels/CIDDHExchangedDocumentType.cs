namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDHExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDHExchangedDocumentType
    {

        private IDType idField;

        private TextType[] nameField;

        private DocumentCodeType typeCodeField;

        private DocumentStatusCodeType statusCodeField;

        private DateTimeType issueDateTimeField;

        private IndicatorType copyIndicatorField;

        private TextType purposeField;

        private IndicatorType controlRequirementIndicatorField;

        private MessageFunctionCodeType[] purposeCodeField;

        private ResponseTypeCodeType[] requestedResponseTypeCodeField;

        private IDType versionIDField;

        private CodeType categoryCodeField;

        private CodeType urgencyCodeField;

        private CodeType subtypeCodeField;

        private CINoteType[] includedCINoteField;

        private CITradePartyType issuerCITradePartyField;

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

        public DocumentCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DocumentStatusCodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
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

        public IndicatorType ControlRequirementIndicator
        {
            get => this.controlRequirementIndicatorField;
            set => this.controlRequirementIndicatorField = value;
        }

        [XmlElement("PurposeCode")]
        public MessageFunctionCodeType[] PurposeCode
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

        public IDType VersionID
        {
            get => this.versionIDField;
            set => this.versionIDField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public CodeType UrgencyCode
        {
            get => this.urgencyCodeField;
            set => this.urgencyCodeField = value;
        }

        public CodeType SubtypeCode
        {
            get => this.subtypeCodeField;
            set => this.subtypeCodeField = value;
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

        [XmlElement("AttachedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] AttachedSpecifiedBinaryFile
        {
            get => this.attachedSpecifiedBinaryFileField;
            set => this.attachedSpecifiedBinaryFileField = value;
        }
    }
}