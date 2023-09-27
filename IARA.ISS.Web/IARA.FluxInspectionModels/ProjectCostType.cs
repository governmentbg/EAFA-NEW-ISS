namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectCost", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectCostType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private CostManagementCodeType costManagementTypeCodeField;

        private CostReportingCodeType costReportingTypeCodeField;

        private AmountType componentAmountField;

        private DateType effectiveDateField;

        private QuantityType hoursComponentQuantityField;

        private QuantityType equivalentHeadsComponentQuantityField;

        private QuantityType materialComponentQuantityField;

        private TextType materialComponentNameField;

        private IndicatorType recurringIndicatorField;

        private ProjectResourceType includedProjectResourceField;

        private ProjectPeriodType reportingProjectPeriodField;

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

        public CostManagementCodeType CostManagementTypeCode
        {
            get => this.costManagementTypeCodeField;
            set => this.costManagementTypeCodeField = value;
        }

        public CostReportingCodeType CostReportingTypeCode
        {
            get => this.costReportingTypeCodeField;
            set => this.costReportingTypeCodeField = value;
        }

        public AmountType ComponentAmount
        {
            get => this.componentAmountField;
            set => this.componentAmountField = value;
        }

        public DateType EffectiveDate
        {
            get => this.effectiveDateField;
            set => this.effectiveDateField = value;
        }

        public QuantityType HoursComponentQuantity
        {
            get => this.hoursComponentQuantityField;
            set => this.hoursComponentQuantityField = value;
        }

        public QuantityType EquivalentHeadsComponentQuantity
        {
            get => this.equivalentHeadsComponentQuantityField;
            set => this.equivalentHeadsComponentQuantityField = value;
        }

        public QuantityType MaterialComponentQuantity
        {
            get => this.materialComponentQuantityField;
            set => this.materialComponentQuantityField = value;
        }

        public TextType MaterialComponentName
        {
            get => this.materialComponentNameField;
            set => this.materialComponentNameField = value;
        }

        public IndicatorType RecurringIndicator
        {
            get => this.recurringIndicatorField;
            set => this.recurringIndicatorField = value;
        }

        public ProjectResourceType IncludedProjectResource
        {
            get => this.includedProjectResourceField;
            set => this.includedProjectResourceField = value;
        }

        public ProjectPeriodType ReportingProjectPeriod
        {
            get => this.reportingProjectPeriodField;
            set => this.reportingProjectPeriodField = value;
        }
    }
}