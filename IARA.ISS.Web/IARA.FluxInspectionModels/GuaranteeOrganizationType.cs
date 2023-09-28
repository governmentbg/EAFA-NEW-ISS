namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GuaranteeOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GuaranteeOrganizationType
    {

        private TextType nameField;

        private IDType idField;

        private LocalOrganizationType[] subordinateLocalOrganizationField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("SubordinateLocalOrganization")]
        public LocalOrganizationType[] SubordinateLocalOrganization
        {
            get => this.subordinateLocalOrganizationField;
            set => this.subordinateLocalOrganizationField = value;
        }
    }
}