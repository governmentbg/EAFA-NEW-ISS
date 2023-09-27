namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectDocumentType
    {

        private IDType idField;

        private DocumentCodeType typeCodeField;

        private TextType nameField;

        private TextType purposeField;

        private TextType descriptionField;

        private DateTimeType issueDateTimeField;

        private DateTimeType submissionDateTimeField;

        private DateTimeType receiptDateTimeField;

        private IndicatorType controlRequirementIndicatorField;

        private DateTimeType creationDateTimeField;

        private DocumentStatusCodeType copyStatusCodeField;

        private IndicatorType copyIndicatorField;

        private DateTimeType responseDateTimeField;

        private ProjectPeriodType effectiveProjectPeriodField;

        private ProjectPeriodType acceptableProjectPeriodField;

        private ProjectDocumentType[] referenceProjectDocumentField;

        private ProjectPartyType issuerProjectPartyField;

        private ProjectPartyType ownerProjectPartyField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public DocumentCodeType TypeCode
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

        public TextType Description
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

        public DocumentStatusCodeType CopyStatusCode
        {
            get => this.copyStatusCodeField;
            set => this.copyStatusCodeField = value;
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

        public ProjectPeriodType EffectiveProjectPeriod
        {
            get => this.effectiveProjectPeriodField;
            set => this.effectiveProjectPeriodField = value;
        }

        public ProjectPeriodType AcceptableProjectPeriod
        {
            get => this.acceptableProjectPeriodField;
            set => this.acceptableProjectPeriodField = value;
        }

        [XmlElement("ReferenceProjectDocument")]
        public ProjectDocumentType[] ReferenceProjectDocument
        {
            get => this.referenceProjectDocumentField;
            set => this.referenceProjectDocumentField = value;
        }

        public ProjectPartyType IssuerProjectParty
        {
            get => this.issuerProjectPartyField;
            set => this.issuerProjectPartyField = value;
        }

        public ProjectPartyType OwnerProjectParty
        {
            get => this.ownerProjectPartyField;
            set => this.ownerProjectPartyField = value;
        }
    }
}