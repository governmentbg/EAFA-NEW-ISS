namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSReferencedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSReferencedDocumentType
    {

        private string issueDateTimeField;

        private DocumentCodeType typeCodeField;

        private ReferenceCodeType relationshipTypeCodeField;

        private IDType idField;

        private BinaryObjectType[] attachmentBinaryObjectField;

        private TextType informationField;

        public string IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public DocumentCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public ReferenceCodeType RelationshipTypeCode
        {
            get => this.relationshipTypeCodeField;
            set => this.relationshipTypeCodeField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("AttachmentBinaryObject")]
        public BinaryObjectType[] AttachmentBinaryObject
        {
            get => this.attachmentBinaryObjectField;
            set => this.attachmentBinaryObjectField = value;
        }

        public TextType Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }
    }
}