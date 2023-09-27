namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProgressMonitoredProject", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProgressMonitoredProjectType
    {

        private IDType idField;

        private IDType contractIDField;

        private IDType[] projectPortfolioIDField;

        private TextType descriptionField;

        private TextType nameField;

        private ProjectTypeCodeType typeCodeField;

        private IndicatorType authorityConstraintIndicatorField;

        private IDType[] subProjectIDField;

        private ProjectLocationType[] physicalProjectLocationField;

        private ProjectPeriodType baselinePlanProjectPeriodField;

        private ProjectPeriodType actualPlanProjectPeriodField;

        private ProjectPeriodType estimatedPlanProjectPeriodField;

        private ProjectPeriodType reportingPlanProjectPeriodField;

        private ProjectScheduleCalendarType[] baselineProjectScheduleCalendarField;

        private ProjectReportingCalendarType[] baselineProjectReportingCalendarField;

        private ProjectCostType[] managementReserveProjectCostField;

        private ProjectCostType[] undistributedBudgetProjectCostField;

        private ProjectCostType[] overheadProjectCostField;

        private ProjectCostType[] fundsBorrowedProjectCostField;

        private ProjectCostType[] generalAdministrativeOverheadProjectCostField;

        private ProjectCostType[] performanceMeasurementBaselineTotalProjectCostField;

        private ProjectCostType[] allocatedBudgetTotalProjectCostField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType ContractID
        {
            get => this.contractIDField;
            set => this.contractIDField = value;
        }

        [XmlElement("ProjectPortfolioID")]
        public IDType[] ProjectPortfolioID
        {
            get => this.projectPortfolioIDField;
            set => this.projectPortfolioIDField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public ProjectTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public IndicatorType AuthorityConstraintIndicator
        {
            get => this.authorityConstraintIndicatorField;
            set => this.authorityConstraintIndicatorField = value;
        }

        [XmlElement("SubProjectID")]
        public IDType[] SubProjectID
        {
            get => this.subProjectIDField;
            set => this.subProjectIDField = value;
        }

        [XmlElement("PhysicalProjectLocation")]
        public ProjectLocationType[] PhysicalProjectLocation
        {
            get => this.physicalProjectLocationField;
            set => this.physicalProjectLocationField = value;
        }

        public ProjectPeriodType BaselinePlanProjectPeriod
        {
            get => this.baselinePlanProjectPeriodField;
            set => this.baselinePlanProjectPeriodField = value;
        }

        public ProjectPeriodType ActualPlanProjectPeriod
        {
            get => this.actualPlanProjectPeriodField;
            set => this.actualPlanProjectPeriodField = value;
        }

        public ProjectPeriodType EstimatedPlanProjectPeriod
        {
            get => this.estimatedPlanProjectPeriodField;
            set => this.estimatedPlanProjectPeriodField = value;
        }

        public ProjectPeriodType ReportingPlanProjectPeriod
        {
            get => this.reportingPlanProjectPeriodField;
            set => this.reportingPlanProjectPeriodField = value;
        }

        [XmlElement("BaselineProjectScheduleCalendar")]
        public ProjectScheduleCalendarType[] BaselineProjectScheduleCalendar
        {
            get => this.baselineProjectScheduleCalendarField;
            set => this.baselineProjectScheduleCalendarField = value;
        }

        [XmlElement("BaselineProjectReportingCalendar")]
        public ProjectReportingCalendarType[] BaselineProjectReportingCalendar
        {
            get => this.baselineProjectReportingCalendarField;
            set => this.baselineProjectReportingCalendarField = value;
        }

        [XmlElement("ManagementReserveProjectCost")]
        public ProjectCostType[] ManagementReserveProjectCost
        {
            get => this.managementReserveProjectCostField;
            set => this.managementReserveProjectCostField = value;
        }

        [XmlElement("UndistributedBudgetProjectCost")]
        public ProjectCostType[] UndistributedBudgetProjectCost
        {
            get => this.undistributedBudgetProjectCostField;
            set => this.undistributedBudgetProjectCostField = value;
        }

        [XmlElement("OverheadProjectCost")]
        public ProjectCostType[] OverheadProjectCost
        {
            get => this.overheadProjectCostField;
            set => this.overheadProjectCostField = value;
        }

        [XmlElement("FundsBorrowedProjectCost")]
        public ProjectCostType[] FundsBorrowedProjectCost
        {
            get => this.fundsBorrowedProjectCostField;
            set => this.fundsBorrowedProjectCostField = value;
        }

        [XmlElement("GeneralAdministrativeOverheadProjectCost")]
        public ProjectCostType[] GeneralAdministrativeOverheadProjectCost
        {
            get => this.generalAdministrativeOverheadProjectCostField;
            set => this.generalAdministrativeOverheadProjectCostField = value;
        }

        [XmlElement("PerformanceMeasurementBaselineTotalProjectCost")]
        public ProjectCostType[] PerformanceMeasurementBaselineTotalProjectCost
        {
            get => this.performanceMeasurementBaselineTotalProjectCostField;
            set => this.performanceMeasurementBaselineTotalProjectCostField = value;
        }

        [XmlElement("AllocatedBudgetTotalProjectCost")]
        public ProjectCostType[] AllocatedBudgetTotalProjectCost
        {
            get => this.allocatedBudgetTotalProjectCostField;
            set => this.allocatedBudgetTotalProjectCostField = value;
        }
    }
}