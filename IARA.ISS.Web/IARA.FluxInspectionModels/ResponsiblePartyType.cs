namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ResponsibleParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ResponsiblePartyType
    {

        private TextType organizationNameField;

        private TextType subordinateOrganizationNameField;

        private TextType personNameField;

        private ResponsiblePartyContactType definedResponsiblePartyContactField;

        private UnstructuredAddressType specifiedUnstructuredAddressField;

        public TextType OrganizationName
        {
            get => this.organizationNameField;
            set => this.organizationNameField = value;
        }

        public TextType SubordinateOrganizationName
        {
            get => this.subordinateOrganizationNameField;
            set => this.subordinateOrganizationNameField = value;
        }

        public TextType PersonName
        {
            get => this.personNameField;
            set => this.personNameField = value;
        }

        public ResponsiblePartyContactType DefinedResponsiblePartyContact
        {
            get => this.definedResponsiblePartyContactField;
            set => this.definedResponsiblePartyContactField = value;
        }

        public UnstructuredAddressType SpecifiedUnstructuredAddress
        {
            get => this.specifiedUnstructuredAddressField;
            set => this.specifiedUnstructuredAddressField = value;
        }
    }
}