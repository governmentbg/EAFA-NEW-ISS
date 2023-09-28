namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DelegateeOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DelegateeOrganizationType
    {

        private IDType idField;

        private TextType nameField;

        private TenderingContactType[] designatedTenderingContactField;

        private TenderingAddressType postalTenderingAddressField;

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

        [XmlElement("DesignatedTenderingContact")]
        public TenderingContactType[] DesignatedTenderingContact
        {
            get => this.designatedTenderingContactField;
            set => this.designatedTenderingContactField = value;
        }

        public TenderingAddressType PostalTenderingAddress
        {
            get => this.postalTenderingAddressField;
            set => this.postalTenderingAddressField = value;
        }
    }
}