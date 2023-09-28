namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWPerson", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWPersonType
    {

        private TextType titleField;

        private TextType givenNameField;

        private TextType middleNameField;

        private TextType familyNamePrefixField;

        private TextType familyNameField;

        private TextType nameSuffixField;

        private CodeType genderCodeField;

        public TextType Title
        {
            get => this.titleField;
            set => this.titleField = value;
        }

        public TextType GivenName
        {
            get => this.givenNameField;
            set => this.givenNameField = value;
        }

        public TextType MiddleName
        {
            get => this.middleNameField;
            set => this.middleNameField = value;
        }

        public TextType FamilyNamePrefix
        {
            get => this.familyNamePrefixField;
            set => this.familyNamePrefixField = value;
        }

        public TextType FamilyName
        {
            get => this.familyNameField;
            set => this.familyNameField = value;
        }

        public TextType NameSuffix
        {
            get => this.nameSuffixField;
            set => this.nameSuffixField = value;
        }

        public CodeType GenderCode
        {
            get => this.genderCodeField;
            set => this.genderCodeField = value;
        }
    }
}