namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProgressMonitoredProjectPortfolio", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProgressMonitoredProjectPortfolioType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private TextType categoryField;

        private ProgressMonitoredProjectType[] componentProgressMonitoredProjectField;

        private ProjectCostType[] managementReserveProjectCostField;

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

        public TextType Category
        {
            get => this.categoryField;
            set => this.categoryField = value;
        }

        [XmlElement("ComponentProgressMonitoredProject")]
        public ProgressMonitoredProjectType[] ComponentProgressMonitoredProject
        {
            get => this.componentProgressMonitoredProjectField;
            set => this.componentProgressMonitoredProjectField = value;
        }

        [XmlElement("ManagementReserveProjectCost")]
        public ProjectCostType[] ManagementReserveProjectCost
        {
            get => this.managementReserveProjectCostField;
            set => this.managementReserveProjectCostField = value;
        }
    }
}