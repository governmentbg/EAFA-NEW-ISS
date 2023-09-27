namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReportedHierarchicalStructure", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReportedHierarchicalStructureType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private HierarchicalStructureTypeCodeType typeCodeField;

        private PlanningLevelCodeType planningLevelCodeField;

        private ReportingDataNodeType[] componentReportingDataNodeField;

        private ProgressMonitoredProjectType[] usedProgressMonitoredProjectField;

        private ProjectCostType[] associatedProjectCostField;

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

        public HierarchicalStructureTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public PlanningLevelCodeType PlanningLevelCode
        {
            get => this.planningLevelCodeField;
            set => this.planningLevelCodeField = value;
        }

        [XmlElement("ComponentReportingDataNode")]
        public ReportingDataNodeType[] ComponentReportingDataNode
        {
            get => this.componentReportingDataNodeField;
            set => this.componentReportingDataNodeField = value;
        }

        [XmlElement("UsedProgressMonitoredProject")]
        public ProgressMonitoredProjectType[] UsedProgressMonitoredProject
        {
            get => this.usedProgressMonitoredProjectField;
            set => this.usedProgressMonitoredProjectField = value;
        }

        [XmlElement("AssociatedProjectCost")]
        public ProjectCostType[] AssociatedProjectCost
        {
            get => this.associatedProjectCostField;
            set => this.associatedProjectCostField = value;
        }
    }
}