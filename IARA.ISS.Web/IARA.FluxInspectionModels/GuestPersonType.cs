namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GuestPerson", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GuestPersonType
    {

        private IDType idField;

        private TextType[] nameField;

        private TextType[] titleField;

        private CodeType genderCodeField;

        private FormattedDateTimeType birthDateTimeField;

        private IDType languageIDField;

        private TextType[] descriptionField;

        private IDType passportIDField;

        private UniversalCommunicationType[] usedUniversalCommunicationField;

        private StructuredAddressType informationStructuredAddressField;

        private ReferencedCountryType nationalityReferencedCountryField;

        private TravelMembershipType[] heldTravelMembershipField;

        private PersonNoteType[] additionalPersonNoteField;

        public IDType ID
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

        [XmlElement("Title")]
        public TextType[] Title
        {
            get => this.titleField;
            set => this.titleField = value;
        }

        public CodeType GenderCode
        {
            get => this.genderCodeField;
            set => this.genderCodeField = value;
        }

        public FormattedDateTimeType BirthDateTime
        {
            get => this.birthDateTimeField;
            set => this.birthDateTimeField = value;
        }

        public IDType LanguageID
        {
            get => this.languageIDField;
            set => this.languageIDField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public IDType PassportID
        {
            get => this.passportIDField;
            set => this.passportIDField = value;
        }

        [XmlElement("UsedUniversalCommunication")]
        public UniversalCommunicationType[] UsedUniversalCommunication
        {
            get => this.usedUniversalCommunicationField;
            set => this.usedUniversalCommunicationField = value;
        }

        public StructuredAddressType InformationStructuredAddress
        {
            get => this.informationStructuredAddressField;
            set => this.informationStructuredAddressField = value;
        }

        public ReferencedCountryType NationalityReferencedCountry
        {
            get => this.nationalityReferencedCountryField;
            set => this.nationalityReferencedCountryField = value;
        }

        [XmlElement("HeldTravelMembership")]
        public TravelMembershipType[] HeldTravelMembership
        {
            get => this.heldTravelMembershipField;
            set => this.heldTravelMembershipField = value;
        }

        [XmlElement("AdditionalPersonNote")]
        public PersonNoteType[] AdditionalPersonNote
        {
            get => this.additionalPersonNoteField;
            set => this.additionalPersonNoteField = value;
        }
    }
}