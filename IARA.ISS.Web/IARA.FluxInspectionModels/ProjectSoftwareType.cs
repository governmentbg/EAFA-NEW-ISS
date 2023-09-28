namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectSoftware", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectSoftwareType
    {

        private TextType nameField;

        private IDType versionIDField;

        private TextType authorNameField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public IDType VersionID
        {
            get => this.versionIDField;
            set => this.versionIDField = value;
        }

        public TextType AuthorName
        {
            get => this.authorNameField;
            set => this.authorNameField = value;
        }
    }
}