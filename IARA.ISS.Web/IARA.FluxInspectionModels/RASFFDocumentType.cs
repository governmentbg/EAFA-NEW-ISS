namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFDocumentType
    {

        private TextType nameField;

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private IDType uRIIDField;

        private CodeType contentTypeCodeField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public IDType URIID
        {
            get => this.uRIIDField;
            set => this.uRIIDField = value;
        }

        public CodeType ContentTypeCode
        {
            get => this.contentTypeCodeField;
            set => this.contentTypeCodeField = value;
        }
    }
}