namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWOrganizationType
    {

        private IDType[] idField;

        private TextType nameField;

        private TMWContactType primaryTMWContactField;

        private StructuredAddressType postalStructuredAddressField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TMWContactType PrimaryTMWContact
        {
            get => this.primaryTMWContactField;
            set => this.primaryTMWContactField = value;
        }

        public StructuredAddressType PostalStructuredAddress
        {
            get => this.postalStructuredAddressField;
            set => this.postalStructuredAddressField = value;
        }
    }
}