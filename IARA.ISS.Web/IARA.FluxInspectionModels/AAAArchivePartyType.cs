namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAArchiveParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAArchivePartyType
    {

        private IDType idField;

        private PartyTypeCodeType typeCodeField;

        private TextType nameField;

        private AccessRightsTypeCodeType accessRightsCodeField;

        private ChargePayingPartyRoleCodeType roleCodeField;

        private LanguageCodeType[] languageCodeField;

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

        public PartyTypeCodeType TypeCode
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

        public AccessRightsTypeCodeType AccessRightsCode
        {
            get
            {
                return this.accessRightsCodeField;
            }
            set
            {
                this.accessRightsCodeField = value;
            }
        }

        public ChargePayingPartyRoleCodeType RoleCode
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

        [XmlElement("LanguageCode")]
        public LanguageCodeType[] LanguageCode
        {
            get
            {
                return this.languageCodeField;
            }
            set
            {
                this.languageCodeField = value;
            }
        }
    }
}