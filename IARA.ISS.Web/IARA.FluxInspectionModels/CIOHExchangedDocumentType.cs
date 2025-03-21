namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOHExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOHExchangedDocumentType
    {

        private IDType idField;

        private TextType[] nameField;

        private DocumentCodeType typeCodeField;

        private DateTimeType issueDateTimeField;

        private IndicatorType copyIndicatorField;

        private IDType[] languageIDField;

        private MessageFunctionCodeType purposeCodeField;

        private DateTimeType revisionDateTimeField;

        private IDType versionIDField;

        private IDType globalIDField;

        private IDType revisionIDField;

        private IDType previousRevisionIDField;

        private CodeType categoryCodeField;

        private CodeType responseReasonCodeField;

        private DocumentStatusCodeType statusCodeField;

        private IDType[] traderAssignedIDField;

        private IDType[] recipientAssignedIDField;

        private CodeType urgencyCodeField;

        private TextType[] urgencyField;

        private TextType[] offsetProcessingStatusField;

        private CINoteType[] includedCINoteField;

        private CISpecifiedPeriodType effectiveCISpecifiedPeriodField;

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

        public CodeType ResponseReasonCode
        {
            get => this.responseReasonCodeField;
            set => this.responseReasonCodeField = value;
        }

        public DocumentStatusCodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        [XmlElement("TraderAssignedID")]
        public IDType[] TraderAssignedID
        {
            get => this.traderAssignedIDField;
            set => this.traderAssignedIDField = value;
        }

        [XmlElement("RecipientAssignedID")]
        public IDType[] RecipientAssignedID
        {
            get => this.recipientAssignedIDField;
            set => this.recipientAssignedIDField = value;
        }

        public CodeType UrgencyCode
        {
            get => this.urgencyCodeField;
            set => this.urgencyCodeField = value;
        }

        [XmlElement("Urgency")]
        public TextType[] Urgency
        {
            get => this.urgencyField;
            set => this.urgencyField = value;
        }

        [XmlElement("OffsetProcessingStatus")]
        public TextType[] OffsetProcessingStatus
        {
            get => this.offsetProcessingStatusField;
            set => this.offsetProcessingStatusField = value;
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

        [XmlElement("AttachedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] AttachedSpecifiedBinaryFile
        {
            get => this.attachedSpecifiedBinaryFileField;
            set => this.attachedSpecifiedBinaryFileField = value;
        }
    }
}