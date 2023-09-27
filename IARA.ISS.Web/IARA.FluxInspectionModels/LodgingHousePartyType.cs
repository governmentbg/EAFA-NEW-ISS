namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHousePartyType
    {

        private CodeType roleCodeField;

        private IDType idField;

        private CodeType[] typeCodeField;

        private TextType[] nameField;

        private TextType[] descriptionField;

        private CodeType accessRightsCodeField;

        private CodeType classificationCodeField;

        private CodeType[] languageCodeField;

        private LodgingHouseContactType definedLodgingHouseContactField;

        public CodeType RoleCode
        {
            get => this.roleCodeField;
            set => this.roleCodeField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public CodeType AccessRightsCode
        {
            get => this.accessRightsCodeField;
            set => this.accessRightsCodeField = value;
        }

        public CodeType ClassificationCode
        {
            get => this.classificationCodeField;
            set => this.classificationCodeField = value;
        }

        [XmlElement("LanguageCode")]
        public CodeType[] LanguageCode
        {
            get => this.languageCodeField;
            set => this.languageCodeField = value;
        }

        public LodgingHouseContactType DefinedLodgingHouseContact
        {
            get => this.definedLodgingHouseContactField;
            set => this.definedLodgingHouseContactField = value;
        }
    }
}