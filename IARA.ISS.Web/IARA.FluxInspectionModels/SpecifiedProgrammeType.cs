namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedProgramme", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedProgrammeType
    {

        private IDType idField;

        private TextType nameField;

        private ProjectTypeCodeType[] typeCodeField;

        private TextType descriptionField;

        private TextType[] sponsorNameField;

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

        [XmlElement("TypeCode")]
        public ProjectTypeCodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("SponsorName")]
        public TextType[] SponsorName
        {
            get => this.sponsorNameField;
            set => this.sponsorNameField = value;
        }
    }
}