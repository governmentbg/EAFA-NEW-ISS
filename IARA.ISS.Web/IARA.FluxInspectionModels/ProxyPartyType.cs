namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProxyParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProxyPartyType
    {

        private IDType idField;

        private CodeType classificationCodeField;

        private TextType nameField;

        private CodeType typeCodeField;

        private TextType descriptionField;

        private CodeType roleCodeField;

        private UnstructuredAddressType postalUnstructuredAddressField;

        private PartyContactType[] definedPartyContactField;

        private ClientFinancialAccountType ownedClientFinancialAccountField;

        private ClientBusinessAccountType specifiedClientBusinessAccountField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType ClassificationCode
        {
            get => this.classificationCodeField;
            set => this.classificationCodeField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public CodeType RoleCode
        {
            get => this.roleCodeField;
            set => this.roleCodeField = value;
        }

        public UnstructuredAddressType PostalUnstructuredAddress
        {
            get => this.postalUnstructuredAddressField;
            set => this.postalUnstructuredAddressField = value;
        }

        [XmlElement("DefinedPartyContact")]
        public PartyContactType[] DefinedPartyContact
        {
            get => this.definedPartyContactField;
            set => this.definedPartyContactField = value;
        }

        public ClientFinancialAccountType OwnedClientFinancialAccount
        {
            get => this.ownedClientFinancialAccountField;
            set => this.ownedClientFinancialAccountField = value;
        }

        public ClientBusinessAccountType SpecifiedClientBusinessAccount
        {
            get => this.specifiedClientBusinessAccountField;
            set => this.specifiedClientBusinessAccountField = value;
        }
    }
}