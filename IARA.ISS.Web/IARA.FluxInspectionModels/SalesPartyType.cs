namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SalesParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SalesPartyType
    {

        private IDType idField;

        private TextType nameField;

        private CodeType typeCodeField;

        private IDType countryIDField;

        private CodeType[] roleCodeField;

        private StructuredAddressType[] specifiedStructuredAddressField;

        private FLUXOrganizationType specifiedFLUXOrganizationField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public IDType CountryID
        {
            get
            {
                return this.countryIDField;
            }
            set
            {
                this.countryIDField = value;
            }
        }

        [XmlElement("RoleCode")]
        public CodeType[] RoleCode
        {
            get
            {
                return this.roleCodeField;
            }
            set
            {
                this.roleCodeField = value;
            }
        }

        [XmlElement("SpecifiedStructuredAddress")]
        public StructuredAddressType[] SpecifiedStructuredAddress
        {
            get
            {
                return this.specifiedStructuredAddressField;
            }
            set
            {
                this.specifiedStructuredAddressField = value;
            }
        }

        public FLUXOrganizationType SpecifiedFLUXOrganization
        {
            get
            {
                return this.specifiedFLUXOrganizationField;
            }
            set
            {
                this.specifiedFLUXOrganizationField = value;
            }
        }
    }
}