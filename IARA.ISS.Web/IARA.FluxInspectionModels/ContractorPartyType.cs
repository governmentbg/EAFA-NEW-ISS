namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ContractorParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ContractorPartyType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private CodeType accessRightsCodeField;

        private CodeType[] languageCodeField;

        private ContractorTypeCodeType typeCodeField;

        private ProjectOrganizationType specifiedProjectOrganizationField;

        private ProjectPersonType specifiedProjectPersonField;

        private UnstructuredAddressType specifiedUnstructuredAddressField;

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

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public CodeType AccessRightsCode
        {
            get => this.accessRightsCodeField;
            set => this.accessRightsCodeField = value;
        }

        [XmlElement("LanguageCode")]
        public CodeType[] LanguageCode
        {
            get => this.languageCodeField;
            set => this.languageCodeField = value;
        }

        public ContractorTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public ProjectOrganizationType SpecifiedProjectOrganization
        {
            get => this.specifiedProjectOrganizationField;
            set => this.specifiedProjectOrganizationField = value;
        }

        public ProjectPersonType SpecifiedProjectPerson
        {
            get => this.specifiedProjectPersonField;
            set => this.specifiedProjectPersonField = value;
        }

        public UnstructuredAddressType SpecifiedUnstructuredAddress
        {
            get => this.specifiedUnstructuredAddressField;
            set => this.specifiedUnstructuredAddressField = value;
        }
    }
}