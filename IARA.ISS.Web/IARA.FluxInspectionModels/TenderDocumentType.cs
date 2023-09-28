namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderDocumentType
    {

        private IDType idField;

        private TextType nameField;

        private DateTimeType submissionDateTimeField;

        private BinaryObjectType[] billOfQuantitiesAttachmentBinaryObjectField;

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

        [XmlElement("BillOfQuantitiesAttachmentBinaryObject")]
        public BinaryObjectType[] BillOfQuantitiesAttachmentBinaryObject
        {
            get => this.billOfQuantitiesAttachmentBinaryObjectField;
            set => this.billOfQuantitiesAttachmentBinaryObjectField = value;
        }
    }
}