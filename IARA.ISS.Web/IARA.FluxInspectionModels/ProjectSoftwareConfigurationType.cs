namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectSoftwareConfiguration", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectSoftwareConfigurationType
    {

        private TextType softwareNameField;

        private TextType[] scheduleOptionField;

        private TextType[] resourceOptionField;

        private ProjectSoftwareType specifiedProjectSoftwareField;

        public TextType SoftwareName
        {
            get => this.softwareNameField;
            set => this.softwareNameField = value;
        }

        [XmlElement("ScheduleOption")]
        public TextType[] ScheduleOption
        {
            get => this.scheduleOptionField;
            set => this.scheduleOptionField = value;
        }

        [XmlElement("ResourceOption")]
        public TextType[] ResourceOption
        {
            get => this.resourceOptionField;
            set => this.resourceOptionField = value;
        }

        public ProjectSoftwareType SpecifiedProjectSoftware
        {
            get => this.specifiedProjectSoftwareField;
            set => this.specifiedProjectSoftwareField = value;
        }
    }
}