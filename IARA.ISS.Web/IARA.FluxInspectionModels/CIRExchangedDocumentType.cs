namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRExchangedDocumentType
    {

        private IDType globalIDField;

        private IDType previousRevisionIDField;

        private IDType revisionIDField;

        private IDType versionIDField;

        private IDType idField;

        private IDType[] languageIDField;

        private MessageFunctionCodeType purposeCodeField;

        private RemittanceDocumentCodeType typeCodeField;

        private DateTimeType issueDateTimeField;

        private DateTimeType revisionDateTimeField;

        private TextType nameField;

        private TextType purposeField;

        private IndicatorType copyIndicatorField;

        private DateTimeType firstVersionIssueDateTimeField;

        private CISpecifiedPeriodType effectiveCISpecifiedPeriodField;

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

        public MessageFunctionCodeType PurposeCode
        {
            get => this.purposeCodeField;
            set => this.purposeCodeField = value;
        }

        public RemittanceDocumentCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
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

        public IndicatorType CopyIndicator
        {
            get => this.copyIndicatorField;
            set => this.copyIndicatorField = value;
        }

        public DateTimeType FirstVersionIssueDateTime
        {
            get => this.firstVersionIssueDateTimeField;
            set => this.firstVersionIssueDateTimeField = value;
        }

        public CISpecifiedPeriodType EffectiveCISpecifiedPeriod
        {
            get => this.effectiveCISpecifiedPeriodField;
            set => this.effectiveCISpecifiedPeriodField = value;
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get => this.includedCINoteField;
            set => this.includedCINoteField = value;
        }
    }
}