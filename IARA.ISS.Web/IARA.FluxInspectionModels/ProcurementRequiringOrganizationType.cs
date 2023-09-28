namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProcurementRequiringOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProcurementRequiringOrganizationType
    {

        private TextType nameField;

        private TextType internationalNameField;

        private IDType idField;

        private TenderingContactType primaryTenderingContactField;

        private TenderingAddressType postalTenderingAddressField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType InternationalName
        {
            get => this.internationalNameField;
            set => this.internationalNameField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TenderingContactType PrimaryTenderingContact
        {
            get => this.primaryTenderingContactField;
            set => this.primaryTenderingContactField = value;
        }

        public TenderingAddressType PostalTenderingAddress
        {
            get => this.postalTenderingAddressField;
            set => this.postalTenderingAddressField = value;
        }
    }
}