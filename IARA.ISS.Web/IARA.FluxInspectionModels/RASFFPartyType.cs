namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFPartyType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private TextType nameField;

        private TextType[] descriptionField;

        private IDType approvalIDField;

        private StructuredAddressType specifiedStructuredAddressField;

        private RASFFDocumentType referencedRASFFDocumentField;

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

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public IDType ApprovalID
        {
            get => this.approvalIDField;
            set => this.approvalIDField = value;
        }

        public StructuredAddressType SpecifiedStructuredAddress
        {
            get => this.specifiedStructuredAddressField;
            set => this.specifiedStructuredAddressField = value;
        }

        public RASFFDocumentType ReferencedRASFFDocument
        {
            get => this.referencedRASFFDocumentField;
            set => this.referencedRASFFDocumentField = value;
        }
    }
}