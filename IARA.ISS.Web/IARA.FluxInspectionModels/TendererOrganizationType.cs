namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TendererOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TendererOrganizationType
    {

        private CodeType businessTypeCodeField;

        private TextType nameField;

        private TextType internationalNameField;

        private IDType idField;

        private TenderingContactType[] designatedTenderingContactField;

        private DelegateeOrganizationType subordinateDelegateeOrganizationField;

        private TenderingAddressType postalTenderingAddressField;

        public CodeType BusinessTypeCode
        {
            get => this.businessTypeCodeField;
            set => this.businessTypeCodeField = value;
        }

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

        [XmlElement("DesignatedTenderingContact")]
        public TenderingContactType[] DesignatedTenderingContact
        {
            get => this.designatedTenderingContactField;
            set => this.designatedTenderingContactField = value;
        }

        public DelegateeOrganizationType SubordinateDelegateeOrganization
        {
            get => this.subordinateDelegateeOrganizationField;
            set => this.subordinateDelegateeOrganizationField = value;
        }

        public TenderingAddressType PostalTenderingAddress
        {
            get => this.postalTenderingAddressField;
            set => this.postalTenderingAddressField = value;
        }
    }
}