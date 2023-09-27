namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReportingDataNode", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReportingDataNodeType
    {

        private IDType idField;

        private IDType[] associatedCostIDField;

        private IDType[] varianceAnalysisNoteIDField;

        private IDType[] parentNodeIDField;

        private IDType dataStructureIDField;

        private IDType[] crossReferenceIDField;

        private TextType nameField;

        private TextType descriptionField;

        private TextType riskLevelDescriptionField;

        private NumericType hierarchicalLevelNumericField;

        private EarnedValueCalculationMethodCodeType earnedValueCalculationMethodCodeField;

        private IndicatorType summaryExclusionIndicatorField;

        private NumericType sequenceNumericField;

        private IDType controlAccountIDField;

        private PlanningLevelCodeType planningLevelCodeField;

        private PercentType completionPercentField;

        private IDType workPackageIDField;

        private ReportingDataNodeType[] subordinateReportingDataNodeField;

        private ProjectPeriodType effectiveProjectPeriodField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("AssociatedCostID")]
        public IDType[] AssociatedCostID
        {
            get => this.associatedCostIDField;
            set => this.associatedCostIDField = value;
        }

        [XmlElement("VarianceAnalysisNoteID")]
        public IDType[] VarianceAnalysisNoteID
        {
            get => this.varianceAnalysisNoteIDField;
            set => this.varianceAnalysisNoteIDField = value;
        }

        [XmlElement("ParentNodeID")]
        public IDType[] ParentNodeID
        {
            get => this.parentNodeIDField;
            set => this.parentNodeIDField = value;
        }

        public IDType DataStructureID
        {
            get => this.dataStructureIDField;
            set => this.dataStructureIDField = value;
        }

        [XmlElement("CrossReferenceID")]
        public IDType[] CrossReferenceID
        {
            get => this.crossReferenceIDField;
            set => this.crossReferenceIDField = value;
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

        public NumericType HierarchicalLevelNumeric
        {
            get => this.hierarchicalLevelNumericField;
            set => this.hierarchicalLevelNumericField = value;
        }

        public EarnedValueCalculationMethodCodeType EarnedValueCalculationMethodCode
        {
            get => this.earnedValueCalculationMethodCodeField;
            set => this.earnedValueCalculationMethodCodeField = value;
        }

        public IndicatorType SummaryExclusionIndicator
        {
            get => this.summaryExclusionIndicatorField;
            set => this.summaryExclusionIndicatorField = value;
        }

        public NumericType SequenceNumeric
        {
            get => this.sequenceNumericField;
            set => this.sequenceNumericField = value;
        }

        public IDType ControlAccountID
        {
            get => this.controlAccountIDField;
            set => this.controlAccountIDField = value;
        }

        public PlanningLevelCodeType PlanningLevelCode
        {
            get => this.planningLevelCodeField;
            set => this.planningLevelCodeField = value;
        }

        public PercentType CompletionPercent
        {
            get => this.completionPercentField;
            set => this.completionPercentField = value;
        }

        public IDType WorkPackageID
        {
            get => this.workPackageIDField;
            set => this.workPackageIDField = value;
        }

        [XmlElement("SubordinateReportingDataNode")]
        public ReportingDataNodeType[] SubordinateReportingDataNode
        {
            get => this.subordinateReportingDataNodeField;
            set => this.subordinateReportingDataNodeField = value;
        }

        public ProjectPeriodType EffectiveProjectPeriod
        {
            get => this.effectiveProjectPeriodField;
            set => this.effectiveProjectPeriodField = value;
        }
    }
}