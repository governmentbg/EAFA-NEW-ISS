namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectScheduleTask", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectScheduleTaskType
    {

        private IDType idField;

        private IDType[] dataNodeIDField;

        private TextType nameField;

        private TextType descriptionField;

        private TextType riskLevelDescriptionField;

        private PercentType calculatedCompletionPercentField;

        private PercentType assessedCompletionPercentField;

        private NumericType priorityRankingNumericField;

        private EarnedValueCalculationMethodCodeType earnedValueMethodCodeField;

        private NumericType scheduleLevelNumericField;

        private DurationUnitMeasureType freeFloatDurationMeasureField;

        private DurationUnitMeasureType totalFloatDurationMeasureField;

        private NumericType milestoneWeightNumericField;

        private ScheduleTaskTypeCodeType typeCodeField;

        private DurationUnitMeasureType totalDurationMeasureField;

        private DurationUnitMeasureType remainingDurationMeasureField;

        private IndicatorType criticalPathIndicatorField;

        private IndicatorType reserveIndicatorField;

        private DateTimeType completionDateTimeField;

        private DurationUnitMeasureType lagTimeMeasureField;

        private IDType controlAccountIDField;

        private TextType scheduleLevelField;

        private DurationUnitMeasureType originalDurationMeasureField;

        private DurationUnitMeasureType baselineDurationMeasureField;

        private DurationUnitMeasureType bestCaseDurationMeasureField;

        private DurationUnitMeasureType mostLikelyDurationMeasureField;

        private DurationUnitMeasureType worstCaseDurationMeasureField;

        private DurationUnitMeasureType finishVarianceDurationMeasureField;

        private DurationUnitMeasureType startVarianceDurationMeasureField;

        private IDType workPackageIDField;

        private IDType sourceIDField;

        private IDType projectIDField;

        private IDType[] associatedCostIDField;

        private PlanningLevelCodeType planningLevelCodeField;

        private ProjectScheduleCalendarType scheduleProjectScheduleCalendarField;

        private BasePeriodType[] currentScheduledBasePeriodField;

        private BasePeriodType earliestScheduledBasePeriodField;

        private BasePeriodType latestScheduledBasePeriodField;

        private BasePeriodType actualScheduledBasePeriodField;

        private BasePeriodType targetScheduledBasePeriodField;

        private BasePeriodType[] resourceScheduledBasePeriodField;

        private BasePeriodType[] bestCaseScheduledBasePeriodField;

        private BasePeriodType[] mostLikelyScheduledBasePeriodField;

        private BasePeriodType[] worstCaseScheduledBasePeriodField;

        private BasePeriodType estimatedScheduledBasePeriodField;

        private BasePeriodType[] baselineScheduledBasePeriodField;

        private ProjectPeriodType reportingProjectPeriodField;

        private ProjectResourceAssignmentType[] requiredProjectResourceAssignmentField;

        private ScheduleTaskTimingConstraintType[] limitingScheduleTaskTimingConstraintField;

        private ProjectNoteType[] userDefinedProjectNoteField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("DataNodeID")]
        public IDType[] DataNodeID
        {
            get => this.dataNodeIDField;
            set => this.dataNodeIDField = value;
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

        public TextType RiskLevelDescription
        {
            get => this.riskLevelDescriptionField;
            set => this.riskLevelDescriptionField = value;
        }

        public PercentType CalculatedCompletionPercent
        {
            get => this.calculatedCompletionPercentField;
            set => this.calculatedCompletionPercentField = value;
        }

        public PercentType AssessedCompletionPercent
        {
            get => this.assessedCompletionPercentField;
            set => this.assessedCompletionPercentField = value;
        }

        public NumericType PriorityRankingNumeric
        {
            get => this.priorityRankingNumericField;
            set => this.priorityRankingNumericField = value;
        }

        public EarnedValueCalculationMethodCodeType EarnedValueMethodCode
        {
            get => this.earnedValueMethodCodeField;
            set => this.earnedValueMethodCodeField = value;
        }

        public NumericType ScheduleLevelNumeric
        {
            get => this.scheduleLevelNumericField;
            set => this.scheduleLevelNumericField = value;
        }

        public DurationUnitMeasureType FreeFloatDurationMeasure
        {
            get => this.freeFloatDurationMeasureField;
            set => this.freeFloatDurationMeasureField = value;
        }

        public DurationUnitMeasureType TotalFloatDurationMeasure
        {
            get => this.totalFloatDurationMeasureField;
            set => this.totalFloatDurationMeasureField = value;
        }

        public NumericType MilestoneWeightNumeric
        {
            get => this.milestoneWeightNumericField;
            set => this.milestoneWeightNumericField = value;
        }

        public ScheduleTaskTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DurationUnitMeasureType TotalDurationMeasure
        {
            get => this.totalDurationMeasureField;
            set => this.totalDurationMeasureField = value;
        }

        public DurationUnitMeasureType RemainingDurationMeasure
        {
            get => this.remainingDurationMeasureField;
            set => this.remainingDurationMeasureField = value;
        }

        public IndicatorType CriticalPathIndicator
        {
            get => this.criticalPathIndicatorField;
            set => this.criticalPathIndicatorField = value;
        }

        public IndicatorType ReserveIndicator
        {
            get => this.reserveIndicatorField;
            set => this.reserveIndicatorField = value;
        }

        public DateTimeType CompletionDateTime
        {
            get => this.completionDateTimeField;
            set => this.completionDateTimeField = value;
        }

        public DurationUnitMeasureType LagTimeMeasure
        {
            get => this.lagTimeMeasureField;
            set => this.lagTimeMeasureField = value;
        }

        public IDType ControlAccountID
        {
            get => this.controlAccountIDField;
            set => this.controlAccountIDField = value;
        }

        public TextType ScheduleLevel
        {
            get => this.scheduleLevelField;
            set => this.scheduleLevelField = value;
        }

        public DurationUnitMeasureType OriginalDurationMeasure
        {
            get => this.originalDurationMeasureField;
            set => this.originalDurationMeasureField = value;
        }

        public DurationUnitMeasureType BaselineDurationMeasure
        {
            get => this.baselineDurationMeasureField;
            set => this.baselineDurationMeasureField = value;
        }

        public DurationUnitMeasureType BestCaseDurationMeasure
        {
            get => this.bestCaseDurationMeasureField;
            set => this.bestCaseDurationMeasureField = value;
        }

        public DurationUnitMeasureType MostLikelyDurationMeasure
        {
            get => this.mostLikelyDurationMeasureField;
            set => this.mostLikelyDurationMeasureField = value;
        }

        public DurationUnitMeasureType WorstCaseDurationMeasure
        {
            get => this.worstCaseDurationMeasureField;
            set => this.worstCaseDurationMeasureField = value;
        }

        public DurationUnitMeasureType FinishVarianceDurationMeasure
        {
            get => this.finishVarianceDurationMeasureField;
            set => this.finishVarianceDurationMeasureField = value;
        }

        public DurationUnitMeasureType StartVarianceDurationMeasure
        {
            get => this.startVarianceDurationMeasureField;
            set => this.startVarianceDurationMeasureField = value;
        }

        public IDType WorkPackageID
        {
            get => this.workPackageIDField;
            set => this.workPackageIDField = value;
        }

        public IDType SourceID
        {
            get => this.sourceIDField;
            set => this.sourceIDField = value;
        }

        public IDType ProjectID
        {
            get => this.projectIDField;
            set => this.projectIDField = value;
        }

        [XmlElement("AssociatedCostID")]
        public IDType[] AssociatedCostID
        {
            get => this.associatedCostIDField;
            set => this.associatedCostIDField = value;
        }

        public PlanningLevelCodeType PlanningLevelCode
        {
            get => this.planningLevelCodeField;
            set => this.planningLevelCodeField = value;
        }

        public ProjectScheduleCalendarType ScheduleProjectScheduleCalendar
        {
            get => this.scheduleProjectScheduleCalendarField;
            set => this.scheduleProjectScheduleCalendarField = value;
        }

        [XmlElement("CurrentScheduledBasePeriod")]
        public BasePeriodType[] CurrentScheduledBasePeriod
        {
            get => this.currentScheduledBasePeriodField;
            set => this.currentScheduledBasePeriodField = value;
        }

        public BasePeriodType EarliestScheduledBasePeriod
        {
            get => this.earliestScheduledBasePeriodField;
            set => this.earliestScheduledBasePeriodField = value;
        }

        public BasePeriodType LatestScheduledBasePeriod
        {
            get => this.latestScheduledBasePeriodField;
            set => this.latestScheduledBasePeriodField = value;
        }

        public BasePeriodType ActualScheduledBasePeriod
        {
            get => this.actualScheduledBasePeriodField;
            set => this.actualScheduledBasePeriodField = value;
        }

        public BasePeriodType TargetScheduledBasePeriod
        {
            get => this.targetScheduledBasePeriodField;
            set => this.targetScheduledBasePeriodField = value;
        }

        [XmlElement("ResourceScheduledBasePeriod")]
        public BasePeriodType[] ResourceScheduledBasePeriod
        {
            get => this.resourceScheduledBasePeriodField;
            set => this.resourceScheduledBasePeriodField = value;
        }

        [XmlElement("BestCaseScheduledBasePeriod")]
        public BasePeriodType[] BestCaseScheduledBasePeriod
        {
            get => this.bestCaseScheduledBasePeriodField;
            set => this.bestCaseScheduledBasePeriodField = value;
        }

        [XmlElement("MostLikelyScheduledBasePeriod")]
        public BasePeriodType[] MostLikelyScheduledBasePeriod
        {
            get => this.mostLikelyScheduledBasePeriodField;
            set => this.mostLikelyScheduledBasePeriodField = value;
        }

        [XmlElement("WorstCaseScheduledBasePeriod")]
        public BasePeriodType[] WorstCaseScheduledBasePeriod
        {
            get => this.worstCaseScheduledBasePeriodField;
            set => this.worstCaseScheduledBasePeriodField = value;
        }

        public BasePeriodType EstimatedScheduledBasePeriod
        {
            get => this.estimatedScheduledBasePeriodField;
            set => this.estimatedScheduledBasePeriodField = value;
        }

        [XmlElement("BaselineScheduledBasePeriod")]
        public BasePeriodType[] BaselineScheduledBasePeriod
        {
            get => this.baselineScheduledBasePeriodField;
            set => this.baselineScheduledBasePeriodField = value;
        }

        public ProjectPeriodType ReportingProjectPeriod
        {
            get => this.reportingProjectPeriodField;
            set => this.reportingProjectPeriodField = value;
        }

        [XmlElement("RequiredProjectResourceAssignment")]
        public ProjectResourceAssignmentType[] RequiredProjectResourceAssignment
        {
            get => this.requiredProjectResourceAssignmentField;
            set => this.requiredProjectResourceAssignmentField = value;
        }

        [XmlElement("LimitingScheduleTaskTimingConstraint")]
        public ScheduleTaskTimingConstraintType[] LimitingScheduleTaskTimingConstraint
        {
            get => this.limitingScheduleTaskTimingConstraintField;
            set => this.limitingScheduleTaskTimingConstraintField = value;
        }

        [XmlElement("UserDefinedProjectNote")]
        public ProjectNoteType[] UserDefinedProjectNote
        {
            get => this.userDefinedProjectNoteField;
            set => this.userDefinedProjectNoteField = value;
        }
    }
}