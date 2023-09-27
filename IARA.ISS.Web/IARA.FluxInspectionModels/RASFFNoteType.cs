namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFNote", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFNoteType
    {

        private TextType[] contentField;

        private IDType[] idField;

        private TextType[] nameField;

        private CodeType typeCodeField;

        private IDType contactPersonIDField;

        private TextType contactPersonNameField;

        private IDType authorOrganizationIDField;

        private TextType authorOrganizationNameField;

        private RASFFCountryType concernedRASFFCountryField;

        [XmlElement("Content")]
        public TextType[] Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public IDType ContactPersonID
        {
            get => this.contactPersonIDField;
            set => this.contactPersonIDField = value;
        }

        public TextType ContactPersonName
        {
            get => this.contactPersonNameField;
            set => this.contactPersonNameField = value;
        }

        public IDType AuthorOrganizationID
        {
            get => this.authorOrganizationIDField;
            set => this.authorOrganizationIDField = value;
        }

        public TextType AuthorOrganizationName
        {
            get => this.authorOrganizationNameField;
            set => this.authorOrganizationNameField = value;
        }

        public RASFFCountryType ConcernedRASFFCountry
        {
            get => this.concernedRASFFCountryField;
            set => this.concernedRASFFCountryField = value;
        }
    }
}