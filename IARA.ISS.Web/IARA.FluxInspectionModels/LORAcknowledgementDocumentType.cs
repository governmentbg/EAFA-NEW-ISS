namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LORAcknowledgementDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LORAcknowledgementDocumentType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private TextType nameField;

        private DateTimeType issueDateTimeField;

        private DateTimeType submissionDateTimeField;

        private DateTimeType receiptDateTimeField;

        private IndicatorType controlRequirementIndicatorField;

        private DateTimeType creationDateTimeField;

        private StatusCodeType statusCodeField;

        private AcknowledgementCodeType acknowledgementStatusCodeField;

        private IDType itemIdentificationIDField;

        private IndicatorType multipleReferencesIndicatorField;

        private TextType[] reasonInformationField;

        private LORReferencedDocumentType[] referenceLORReferencedDocumentField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
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

        public StatusCodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public AcknowledgementCodeType AcknowledgementStatusCode
        {
            get => this.acknowledgementStatusCodeField;
            set => this.acknowledgementStatusCodeField = value;
        }

        public IDType ItemIdentificationID
        {
            get => this.itemIdentificationIDField;
            set => this.itemIdentificationIDField = value;
        }

        public IndicatorType MultipleReferencesIndicator
        {
            get => this.multipleReferencesIndicatorField;
            set => this.multipleReferencesIndicatorField = value;
        }

        [XmlElement("ReasonInformation")]
        public TextType[] ReasonInformation
        {
            get => this.reasonInformationField;
            set => this.reasonInformationField = value;
        }

        [XmlElement("ReferenceLORReferencedDocument")]
        public LORReferencedDocumentType[] ReferenceLORReferencedDocument
        {
            get => this.referenceLORReferencedDocumentField;
            set => this.referenceLORReferencedDocumentField = value;
        }
    }
}