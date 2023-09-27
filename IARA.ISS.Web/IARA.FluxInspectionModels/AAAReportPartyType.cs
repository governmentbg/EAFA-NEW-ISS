namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportPartyType
    {

        private IDType idField;

        private PartyTypeCodeType typeCodeField;

        private TextType personNameField;

        private AccessRightsTypeCodeType accessRightsCodeField;

        private PartyRoleCodeType roleCodeField;

        private CodeType languageCodeField;

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

        public TextType PersonName
        {
            get
            {
                return this.personNameField;
            }
            set
            {
                this.personNameField = value;
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

        public PartyRoleCodeType RoleCode
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

        public CodeType LanguageCode
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