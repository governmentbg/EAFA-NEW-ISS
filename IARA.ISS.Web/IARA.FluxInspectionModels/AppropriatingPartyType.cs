namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AppropriatingParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AppropriatingPartyType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private CodeType accessRightsCodeField;

        private CodeType[] languageCodeField;

        private ProjectOrganizationType specifiedProjectOrganizationField;

        private ProjectPersonType specifiedProjectPersonField;

        private UnstructuredAddressType specifiedUnstructuredAddressField;

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

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public CodeType AccessRightsCode
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

        [XmlElement("LanguageCode")]
        public CodeType[] LanguageCode
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

        public ProjectOrganizationType SpecifiedProjectOrganization
        {
            get
            {
                return this.specifiedProjectOrganizationField;
            }
            set
            {
                this.specifiedProjectOrganizationField = value;
            }
        }

        public ProjectPersonType SpecifiedProjectPerson
        {
            get
            {
                return this.specifiedProjectPersonField;
            }
            set
            {
                this.specifiedProjectPersonField = value;
            }
        }

        public UnstructuredAddressType SpecifiedUnstructuredAddress
        {
            get
            {
                return this.specifiedUnstructuredAddressField;
            }
            set
            {
                this.specifiedUnstructuredAddressField = value;
            }
        }
    }
}