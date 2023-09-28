namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProcuringProject", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProcuringProjectType
    {

        private IDType idField;

        private TextType descriptionField;

        private TextType nameField;

        private CodeType[] worksTypeCodeField;

        private CodeType[] subWorksTypeCodeField;

        private CodeType typeCodeField;

        private IndicatorType constraintIndicatorField;

        private AmountType totalBudgetAmountField;

        private AmountType netBudgetAmountField;

        private ProjectActualizationLocationType physicalProjectActualizationLocationField;

        private TenderingPeriodType planTenderingPeriodField;

        private InspectionEventType specifiedInspectionEventField;

        private ProjectPeriodType[] planProjectPeriodField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlElement("WorksTypeCode")]
        public CodeType[] WorksTypeCode
        {
            get
            {
                return this.worksTypeCodeField;
            }
            set
            {
                this.worksTypeCodeField = value;
            }
        }

        [XmlElement("SubWorksTypeCode")]
        public CodeType[] SubWorksTypeCode
        {
            get
            {
                return this.subWorksTypeCodeField;
            }
            set
            {
                this.subWorksTypeCodeField = value;
            }
        }

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public IndicatorType ConstraintIndicator
        {
            get
            {
                return this.constraintIndicatorField;
            }
            set
            {
                this.constraintIndicatorField = value;
            }
        }

        public AmountType TotalBudgetAmount
        {
            get
            {
                return this.totalBudgetAmountField;
            }
            set
            {
                this.totalBudgetAmountField = value;
            }
        }

        public AmountType NetBudgetAmount
        {
            get
            {
                return this.netBudgetAmountField;
            }
            set
            {
                this.netBudgetAmountField = value;
            }
        }

        public ProjectActualizationLocationType PhysicalProjectActualizationLocation
        {
            get
            {
                return this.physicalProjectActualizationLocationField;
            }
            set
            {
                this.physicalProjectActualizationLocationField = value;
            }
        }

        public TenderingPeriodType PlanTenderingPeriod
        {
            get
            {
                return this.planTenderingPeriodField;
            }
            set
            {
                this.planTenderingPeriodField = value;
            }
        }

        public InspectionEventType SpecifiedInspectionEvent
        {
            get
            {
                return this.specifiedInspectionEventField;
            }
            set
            {
                this.specifiedInspectionEventField = value;
            }
        }

        [XmlElement("PlanProjectPeriod")]
        public ProjectPeriodType[] PlanProjectPeriod
        {
            get
            {
                return this.planProjectPeriodField;
            }
            set
            {
                this.planProjectPeriodField = value;
            }
        }
    }
}