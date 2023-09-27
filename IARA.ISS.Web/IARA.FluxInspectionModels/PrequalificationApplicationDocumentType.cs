namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PrequalificationApplicationDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PrequalificationApplicationDocumentType
    {

        private IDType idField;

        private TextType nameField;

        private DateTimeType submissionDateTimeField;

        private BinaryObjectType[] attachmentBinaryObjectField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public DateTimeType SubmissionDateTime
        {
            get => this.submissionDateTimeField;
            set => this.submissionDateTimeField = value;
        }

        [XmlElement("AttachmentBinaryObject")]
        public BinaryObjectType[] AttachmentBinaryObject
        {
            get => this.attachmentBinaryObjectField;
            set => this.attachmentBinaryObjectField = value;
        }
    }
}