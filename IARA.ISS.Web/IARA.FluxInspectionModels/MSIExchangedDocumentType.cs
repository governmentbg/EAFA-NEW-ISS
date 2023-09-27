namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MSIExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MSIExchangedDocumentType
    {

        private IDType idField;

        private TextType[] nameField;

        private DocumentCodeType typeCodeField;

        private DocumentStatusCodeType statusCodeField;

        private DateTimeType issueDateTimeField;

        private IndicatorType copyIndicatorField;

        private MessageFunctionCodeType[] purposeCodeField;

        private IDType versionIDField;

        private IDType revisionIDField;

        private IDType previousRevisionIDField;

        private NoteType[] includedNoteField;

        private SpecifiedPeriodType effectiveSpecifiedPeriodField;

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

        [XmlElement("PurposeCode")]
        public MessageFunctionCodeType[] PurposeCode
        {
            get => this.purposeCodeField;
            set => this.purposeCodeField = value;
        }

        public IDType VersionID
        {
            get => this.versionIDField;
            set => this.versionIDField = value;
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

        [XmlElement("IncludedNote")]
        public NoteType[] IncludedNote
        {
            get => this.includedNoteField;
            set => this.includedNoteField = value;
        }

        public SpecifiedPeriodType EffectiveSpecifiedPeriod
        {
            get => this.effectiveSpecifiedPeriodField;
            set => this.effectiveSpecifiedPeriodField = value;
        }
    }
}