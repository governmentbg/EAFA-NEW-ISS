namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSQualification", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSQualificationType
    {

        private TextType[] nameField;

        private TextType abbreviatedNameField;

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType AbbreviatedName
        {
            get => this.abbreviatedNameField;
            set => this.abbreviatedNameField = value;
        }
    }
}