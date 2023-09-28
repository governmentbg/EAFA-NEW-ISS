namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDocumentLineDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDocumentLineDocumentType
    {

        private DateTimeType issueDateTimeField;

        private DateTimeType latestRevisionDateTimeField;

        private IDType lineIDField;

        private LineStatusCodeType lineStatusCodeField;

        private CodeType lineStatusReasonCodeField;

        private TextType[] lineStatusReasonField;

        private IDType parentLineIDField;

        private DateTimeType publicationDateTimeField;

        private IDType uUIDLineIDField;

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public DateTimeType LatestRevisionDateTime
        {
            get => this.latestRevisionDateTimeField;
            set => this.latestRevisionDateTimeField = value;
        }

        public IDType LineID
        {
            get => this.lineIDField;
            set => this.lineIDField = value;
        }

        public LineStatusCodeType LineStatusCode
        {
            get => this.lineStatusCodeField;
            set => this.lineStatusCodeField = value;
        }

        public CodeType LineStatusReasonCode
        {
            get => this.lineStatusReasonCodeField;
            set => this.lineStatusReasonCodeField = value;
        }

        [XmlElement("LineStatusReason")]
        public TextType[] LineStatusReason
        {
            get => this.lineStatusReasonField;
            set => this.lineStatusReasonField = value;
        }

        public IDType ParentLineID
        {
            get => this.parentLineIDField;
            set => this.parentLineIDField = value;
        }

        public DateTimeType PublicationDateTime
        {
            get => this.publicationDateTimeField;
            set => this.publicationDateTimeField = value;
        }

        public IDType UUIDLineID
        {
            get => this.uUIDLineIDField;
            set => this.uUIDLineIDField = value;
        }
    }
}