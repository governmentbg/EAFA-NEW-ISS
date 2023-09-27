namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWPartyType
    {

        private IDType[] idField;

        private IndicatorType branchIndicatorField;

        private TextType organizationNameField;

        private TMWPersonType specifiedTMWPersonField;

        private TMWContactType definedTMWContactField;

        private StructuredAddressType officeStructuredAddressField;

        private TMWOrganizationType principalPlaceOfBusinessTMWOrganizationField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IndicatorType BranchIndicator
        {
            get => this.branchIndicatorField;
            set => this.branchIndicatorField = value;
        }

        public TextType OrganizationName
        {
            get => this.organizationNameField;
            set => this.organizationNameField = value;
        }

        public TMWPersonType SpecifiedTMWPerson
        {
            get => this.specifiedTMWPersonField;
            set => this.specifiedTMWPersonField = value;
        }

        public TMWContactType DefinedTMWContact
        {
            get => this.definedTMWContactField;
            set => this.definedTMWContactField = value;
        }

        public StructuredAddressType OfficeStructuredAddress
        {
            get => this.officeStructuredAddressField;
            set => this.officeStructuredAddressField = value;
        }

        public TMWOrganizationType PrincipalPlaceOfBusinessTMWOrganization
        {
            get => this.principalPlaceOfBusinessTMWOrganizationField;
            set => this.principalPlaceOfBusinessTMWOrganizationField = value;
        }
    }
}