namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectResourceAssignment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectResourceAssignmentType
    {

        private IDType idField;

        private TextType nameField;

        private CostManagementCodeType costManagementCodeField;

        private AmountType resourceAmountField;

        private IndicatorType durationBasedIndicatorField;

        private QuantityType hourResourceQuantityField;

        private QuantityType materialResourceQuantityField;

        private ProjectPeriodType allocatedProjectPeriodField;

        private ProjectResourceType allocatedProjectResourceField;

        private ProjectScheduleCalendarType scheduleProjectScheduleCalendarField;

        private ResourceDistributionProfileType[] allocationResourceDistributionProfileField;

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

        public CostManagementCodeType CostManagementCode
        {
            get => this.costManagementCodeField;
            set => this.costManagementCodeField = value;
        }

        public AmountType ResourceAmount
        {
            get => this.resourceAmountField;
            set => this.resourceAmountField = value;
        }

        public IndicatorType DurationBasedIndicator
        {
            get => this.durationBasedIndicatorField;
            set => this.durationBasedIndicatorField = value;
        }

        public QuantityType HourResourceQuantity
        {
            get => this.hourResourceQuantityField;
            set => this.hourResourceQuantityField = value;
        }

        public QuantityType MaterialResourceQuantity
        {
            get => this.materialResourceQuantityField;
            set => this.materialResourceQuantityField = value;
        }

        public ProjectPeriodType AllocatedProjectPeriod
        {
            get => this.allocatedProjectPeriodField;
            set => this.allocatedProjectPeriodField = value;
        }

        public ProjectResourceType AllocatedProjectResource
        {
            get => this.allocatedProjectResourceField;
            set => this.allocatedProjectResourceField = value;
        }

        public ProjectScheduleCalendarType ScheduleProjectScheduleCalendar
        {
            get => this.scheduleProjectScheduleCalendarField;
            set => this.scheduleProjectScheduleCalendarField = value;
        }

        [XmlElement("AllocationResourceDistributionProfile")]
        public ResourceDistributionProfileType[] AllocationResourceDistributionProfile
        {
            get => this.allocationResourceDistributionProfileField;
            set => this.allocationResourceDistributionProfileField = value;
        }
    }
}