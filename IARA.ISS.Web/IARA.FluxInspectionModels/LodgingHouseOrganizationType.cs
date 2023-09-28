namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseOrganizationType
    {

        private IDType[] idField;

        private TextType[] chainNameField;

        private TextType[] brandNameField;

        private TextType[] nameField;

        private CodeType[] businessTypeCodeField;

        private TextType[] associationMembershipNameField;

        private TextType[] allianceNameField;

        private TextType ownerNameField;

        private TextType descriptionField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("ChainName")]
        public TextType[] ChainName
        {
            get => this.chainNameField;
            set => this.chainNameField = value;
        }

        [XmlElement("BrandName")]
        public TextType[] BrandName
        {
            get => this.brandNameField;
            set => this.brandNameField = value;
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("BusinessTypeCode")]
        public CodeType[] BusinessTypeCode
        {
            get => this.businessTypeCodeField;
            set => this.businessTypeCodeField = value;
        }

        [XmlElement("AssociationMembershipName")]
        public TextType[] AssociationMembershipName
        {
            get => this.associationMembershipNameField;
            set => this.associationMembershipNameField = value;
        }

        [XmlElement("AllianceName")]
        public TextType[] AllianceName
        {
            get => this.allianceNameField;
            set => this.allianceNameField = value;
        }

        public TextType OwnerName
        {
            get => this.ownerNameField;
            set => this.ownerNameField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}