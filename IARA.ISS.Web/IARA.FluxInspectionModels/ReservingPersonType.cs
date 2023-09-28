namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReservingPerson", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReservingPersonType
    {

        private TextType nameField;

        private TextType titleField;

        private IDType languageIDField;

        private TextType descriptionField;

        private UniversalCommunicationType[] usedUniversalCommunicationField;

        private StructuredAddressType informationStructuredAddressField;

        private ReferencedCountryType nationalityReferencedCountryField;

        private TravelMembershipType[] heldTravelMembershipField;

        private PersonNoteType[] additionalPersonNoteField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType Title
        {
            get => this.titleField;
            set => this.titleField = value;
        }

        public IDType LanguageID
        {
            get => this.languageIDField;
            set => this.languageIDField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
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