namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PSCPMAcknowledgementDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PSCPMAcknowledgementDocumentType
    {

        private IndicatorType multipleReferencesIndicatorField;

        private IDType idField;

        private DocumentCodeType[] typeCodeField;

        private TextType nameField;

        private DateTimeType issueDateTimeField;

        private DateTimeType reportSubmissionDateTimeField;

        private DateTimeType reportReceiptDateTimeField;

        private IndicatorType controlRequirementIndicatorField;

        private DateTimeType creationDateTimeField;

        private StatusCodeType[] statusCodeField;

        private AcknowledgementCodeType[] acknowledgementStatusCodeField;

        private IDType itemIdentificationIDField;

        private TextType[] reasonInformationField;

        public IndicatorType MultipleReferencesIndicator
        {
            get => this.multipleReferencesIndicatorField;
            set => this.multipleReferencesIndicatorField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("TypeCode")]
        public DocumentCodeType[] TypeCode
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

        public DateTimeType ReportSubmissionDateTime
        {
            get => this.reportSubmissionDateTimeField;
            set => this.reportSubmissionDateTimeField = value;
        }

        public DateTimeType ReportReceiptDateTime
        {
            get => this.reportReceiptDateTimeField;
            set => this.reportReceiptDateTimeField = value;
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
        public StatusCodeType[] StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        [XmlElement("AcknowledgementStatusCode")]
        public AcknowledgementCodeType[] AcknowledgementStatusCode
        {
            get => this.acknowledgementStatusCodeField;
            set => this.acknowledgementStatusCodeField = value;
        }

        public IDType ItemIdentificationID
        {
            get => this.itemIdentificationIDField;
            set => this.itemIdentificationIDField = value;
        }

        [XmlElement("ReasonInformation")]
        public TextType[] ReasonInformation
        {
            get => this.reasonInformationField;
            set => this.reasonInformationField = value;
        }
    }
}