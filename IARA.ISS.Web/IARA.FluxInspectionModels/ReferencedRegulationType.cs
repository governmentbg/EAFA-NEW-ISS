namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReferencedRegulation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReferencedRegulationType
    {

        private TextType agencyNameField;

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private TextType citationField;

        private CodeType sourceCodeField;

        private IDType originCountryIDField;

        private CodeType scopeCodeField;

        private CodeType languageCodeField;

        public TextType AgencyName
        {
            get => this.agencyNameField;
            set => this.agencyNameField = value;
        }

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

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TextType Citation
        {
            get => this.citationField;
            set => this.citationField = value;
        }

        public CodeType SourceCode
        {
            get => this.sourceCodeField;
            set => this.sourceCodeField = value;
        }

        public IDType OriginCountryID
        {
            get => this.originCountryIDField;
            set => this.originCountryIDField = value;
        }

        public CodeType ScopeCode
        {
            get => this.scopeCodeField;
            set => this.scopeCodeField = value;
        }

        public CodeType LanguageCode
        {
            get => this.languageCodeField;
            set => this.languageCodeField = value;
        }
    }
}