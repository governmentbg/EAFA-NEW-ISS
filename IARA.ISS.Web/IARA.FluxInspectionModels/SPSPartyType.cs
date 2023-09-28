namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSPartyType
    {

        private IDType idField;

        private TextType nameField;

        private PartyRoleCodeType roleCodeField;

        private CodeType[] typeCodeField;

        private SPSAddressType specifiedSPSAddressField;

        private SPSContactType[] definedSPSContactField;

        private SPSPersonType specifiedSPSPersonField;

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

        public PartyRoleCodeType RoleCode
        {
            get => this.roleCodeField;
            set => this.roleCodeField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public SPSAddressType SpecifiedSPSAddress
        {
            get => this.specifiedSPSAddressField;
            set => this.specifiedSPSAddressField = value;
        }

        [XmlElement("DefinedSPSContact")]
        public SPSContactType[] DefinedSPSContact
        {
            get => this.definedSPSContactField;
            set => this.definedSPSContactField = value;
        }

        public SPSPersonType SpecifiedSPSPerson
        {
            get => this.specifiedSPSPersonField;
            set => this.specifiedSPSPersonField = value;
        }
    }
}