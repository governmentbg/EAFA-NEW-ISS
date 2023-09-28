namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LocationResponsibleOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LocationResponsibleOrganizationType
    {

        private IDType[] idField;

        private TextType nameField;

        private CodeType businessTypeCodeField;

        private QuantityType membersAndManagersQuantityField;

        private CodeType legalClassificationCodeField;

        private StructuredAddressType physicalStructuredAddressField;

        private TTPartyType[] tTResponsibleTTPartyField;

        [XmlElement("ID")]
        public IDType[] ID
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

        public CodeType BusinessTypeCode
        {
            get
            {
                return this.businessTypeCodeField;
            }
            set
            {
                this.businessTypeCodeField = value;
            }
        }

        public QuantityType MembersAndManagersQuantity
        {
            get
            {
                return this.membersAndManagersQuantityField;
            }
            set
            {
                this.membersAndManagersQuantityField = value;
            }
        }

        public CodeType LegalClassificationCode
        {
            get
            {
                return this.legalClassificationCodeField;
            }
            set
            {
                this.legalClassificationCodeField = value;
            }
        }

        public StructuredAddressType PhysicalStructuredAddress
        {
            get
            {
                return this.physicalStructuredAddressField;
            }
            set
            {
                this.physicalStructuredAddressField = value;
            }
        }

        [XmlElement("TTResponsibleTTParty")]
        public TTPartyType[] TTResponsibleTTParty
        {
            get
            {
                return this.tTResponsibleTTPartyField;
            }
            set
            {
                this.tTResponsibleTTPartyField = value;
            }
        }
    }
}