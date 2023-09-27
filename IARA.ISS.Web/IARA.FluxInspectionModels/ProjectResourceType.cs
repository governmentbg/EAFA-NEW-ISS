namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectResource", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectResourceType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private ResourcePlanMeasureTypeCodeType planMeasureTypeCodeField;

        private QuantityType availabilityQuantityField;

        private AmountType unitCostAmountField;

        private CodeType unitCostMeasureCodeField;

        private ResourceCostCategoryCodeType costCategoryCodeField;

        private IDType[] crossReferenceIDField;

        private ProjectPeriodType[] availabilityProjectPeriodField;

        private ProjectScheduleCalendarType[] scheduleProjectScheduleCalendarField;

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

        public ResourcePlanMeasureTypeCodeType PlanMeasureTypeCode
        {
            get => this.planMeasureTypeCodeField;
            set => this.planMeasureTypeCodeField = value;
        }

        public QuantityType AvailabilityQuantity
        {
            get => this.availabilityQuantityField;
            set => this.availabilityQuantityField = value;
        }

        public AmountType UnitCostAmount
        {
            get => this.unitCostAmountField;
            set => this.unitCostAmountField = value;
        }

        public CodeType UnitCostMeasureCode
        {
            get => this.unitCostMeasureCodeField;
            set => this.unitCostMeasureCodeField = value;
        }

        public ResourceCostCategoryCodeType CostCategoryCode
        {
            get => this.costCategoryCodeField;
            set => this.costCategoryCodeField = value;
        }

        [XmlElement("CrossReferenceID")]
        public IDType[] CrossReferenceID
        {
            get => this.crossReferenceIDField;
            set => this.crossReferenceIDField = value;
        }

        [XmlElement("AvailabilityProjectPeriod")]
        public ProjectPeriodType[] AvailabilityProjectPeriod
        {
            get => this.availabilityProjectPeriodField;
            set => this.availabilityProjectPeriodField = value;
        }

        [XmlElement("ScheduleProjectScheduleCalendar")]
        public ProjectScheduleCalendarType[] ScheduleProjectScheduleCalendar
        {
            get => this.scheduleProjectScheduleCalendarField;
            set => this.scheduleProjectScheduleCalendarField = value;
        }
    }
}