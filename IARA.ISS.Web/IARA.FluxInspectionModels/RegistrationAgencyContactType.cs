namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RegistrationAgencyContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RegistrationAgencyContactType
    {

        private IDType idField;

        private TextType jobTitleField;

        private TextType governmentalResponsibilityField;

        private TextType personNameField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType JobTitle
        {
            get => this.jobTitleField;
            set => this.jobTitleField = value;
        }

        public TextType GovernmentalResponsibility
        {
            get => this.governmentalResponsibilityField;
            set => this.governmentalResponsibilityField = value;
        }

        public TextType PersonName
        {
            get => this.personNameField;
            set => this.personNameField = value;
        }
    }
}