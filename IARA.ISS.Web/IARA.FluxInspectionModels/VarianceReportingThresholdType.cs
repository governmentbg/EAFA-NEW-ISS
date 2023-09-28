namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VarianceReportingThreshold", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VarianceReportingThresholdType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private ReportingThresholdTriggerTypeCodeType triggerTypeCodeField;

        private CostReportingCodeType costReportingCodeField;

        private AmountType currentCostMinimumVarianceAmountField;

        private AmountType currentScheduleMinimumVarianceAmountField;

        private AmountType cumulativeScheduleMinimumVarianceAmountField;

        private AmountType cumulativeCostMinimumVarianceAmountField;

        private AmountType atCompleteMinimumVarianceAmountField;

        private PercentType currentCostMinimumVariancePercentField;

        private PercentType currentScheduleMinimumVariancePercentField;

        private PercentType cumulativeCostMinimumVariancePercentField;

        private PercentType cumulativeScheduleMinimumVariancePercentField;

        private PercentType atCompleteMinimumVariancePercentField;

        private ReportingDataNodeType[] appliedReportingDataNodeField;

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

        public ReportingThresholdTriggerTypeCodeType TriggerTypeCode
        {
            get => this.triggerTypeCodeField;
            set => this.triggerTypeCodeField = value;
        }

        public CostReportingCodeType CostReportingCode
        {
            get => this.costReportingCodeField;
            set => this.costReportingCodeField = value;
        }

        public AmountType CurrentCostMinimumVarianceAmount
        {
            get => this.currentCostMinimumVarianceAmountField;
            set => this.currentCostMinimumVarianceAmountField = value;
        }

        public AmountType CurrentScheduleMinimumVarianceAmount
        {
            get => this.currentScheduleMinimumVarianceAmountField;
            set => this.currentScheduleMinimumVarianceAmountField = value;
        }

        public AmountType CumulativeScheduleMinimumVarianceAmount
        {
            get => this.cumulativeScheduleMinimumVarianceAmountField;
            set => this.cumulativeScheduleMinimumVarianceAmountField = value;
        }

        public AmountType CumulativeCostMinimumVarianceAmount
        {
            get => this.cumulativeCostMinimumVarianceAmountField;
            set => this.cumulativeCostMinimumVarianceAmountField = value;
        }

        public AmountType AtCompleteMinimumVarianceAmount
        {
            get => this.atCompleteMinimumVarianceAmountField;
            set => this.atCompleteMinimumVarianceAmountField = value;
        }

        public PercentType CurrentCostMinimumVariancePercent
        {
            get => this.currentCostMinimumVariancePercentField;
            set => this.currentCostMinimumVariancePercentField = value;
        }

        public PercentType CurrentScheduleMinimumVariancePercent
        {
            get => this.currentScheduleMinimumVariancePercentField;
            set => this.currentScheduleMinimumVariancePercentField = value;
        }

        public PercentType CumulativeCostMinimumVariancePercent
        {
            get => this.cumulativeCostMinimumVariancePercentField;
            set => this.cumulativeCostMinimumVariancePercentField = value;
        }

        public PercentType CumulativeScheduleMinimumVariancePercent
        {
            get => this.cumulativeScheduleMinimumVariancePercentField;
            set => this.cumulativeScheduleMinimumVariancePercentField = value;
        }

        public PercentType AtCompleteMinimumVariancePercent
        {
            get => this.atCompleteMinimumVariancePercentField;
            set => this.atCompleteMinimumVariancePercentField = value;
        }

        [XmlElement("AppliedReportingDataNode")]
        public ReportingDataNodeType[] AppliedReportingDataNode
        {
            get => this.appliedReportingDataNodeField;
            set => this.appliedReportingDataNodeField = value;
        }
    }
}