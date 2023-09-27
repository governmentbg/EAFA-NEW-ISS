namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAEntryCapitalAssetAmortization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAEntryCapitalAssetAmortizationType
    {

        private MeasureType lifetimeDurationMeasureField;

        private AmountType basisAmountField;

        private QuantityType lifetimeProductionCapacityQuantityField;

        private AmortizationMethodCodeType methodCodeField;

        private IDType methodLegalReferenceIDField;

        private TextType methodLegalReferenceField;

        private AmountType calculationParameterAmountField;

        private NumericType calculationParameterNumericField;

        private PercentType calculationParameterPercentField;

        private AmountType residualValueAmountField;

        private AmountType lifetimeEndCostAmountField;

        private LifetimeEndCostCodeType lifetimeEndCostTypeCodeField;

        private AmountType basisReductionAmountField;

        private PercentType basisReductionPercentField;

        private AmountType maintenanceCostAmountField;

        private QuantityType periodProductionQuantityField;

        private AAAPeriodType specifiedAAAPeriodField;

        public MeasureType LifetimeDurationMeasure
        {
            get
            {
                return this.lifetimeDurationMeasureField;
            }
            set
            {
                this.lifetimeDurationMeasureField = value;
            }
        }

        public AmountType BasisAmount
        {
            get
            {
                return this.basisAmountField;
            }
            set
            {
                this.basisAmountField = value;
            }
        }

        public QuantityType LifetimeProductionCapacityQuantity
        {
            get
            {
                return this.lifetimeProductionCapacityQuantityField;
            }
            set
            {
                this.lifetimeProductionCapacityQuantityField = value;
            }
        }

        public AmortizationMethodCodeType MethodCode
        {
            get
            {
                return this.methodCodeField;
            }
            set
            {
                this.methodCodeField = value;
            }
        }

        public IDType MethodLegalReferenceID
        {
            get
            {
                return this.methodLegalReferenceIDField;
            }
            set
            {
                this.methodLegalReferenceIDField = value;
            }
        }

        public TextType MethodLegalReference
        {
            get
            {
                return this.methodLegalReferenceField;
            }
            set
            {
                this.methodLegalReferenceField = value;
            }
        }

        public AmountType CalculationParameterAmount
        {
            get
            {
                return this.calculationParameterAmountField;
            }
            set
            {
                this.calculationParameterAmountField = value;
            }
        }

        public NumericType CalculationParameterNumeric
        {
            get
            {
                return this.calculationParameterNumericField;
            }
            set
            {
                this.calculationParameterNumericField = value;
            }
        }

        public PercentType CalculationParameterPercent
        {
            get
            {
                return this.calculationParameterPercentField;
            }
            set
            {
                this.calculationParameterPercentField = value;
            }
        }

        public AmountType ResidualValueAmount
        {
            get
            {
                return this.residualValueAmountField;
            }
            set
            {
                this.residualValueAmountField = value;
            }
        }

        public AmountType LifetimeEndCostAmount
        {
            get
            {
                return this.lifetimeEndCostAmountField;
            }
            set
            {
                this.lifetimeEndCostAmountField = value;
            }
        }

        public LifetimeEndCostCodeType LifetimeEndCostTypeCode
        {
            get
            {
                return this.lifetimeEndCostTypeCodeField;
            }
            set
            {
                this.lifetimeEndCostTypeCodeField = value;
            }
        }

        public AmountType BasisReductionAmount
        {
            get
            {
                return this.basisReductionAmountField;
            }
            set
            {
                this.basisReductionAmountField = value;
            }
        }

        public PercentType BasisReductionPercent
        {
            get
            {
                return this.basisReductionPercentField;
            }
            set
            {
                this.basisReductionPercentField = value;
            }
        }

        public AmountType MaintenanceCostAmount
        {
            get
            {
                return this.maintenanceCostAmountField;
            }
            set
            {
                this.maintenanceCostAmountField = value;
            }
        }

        public QuantityType PeriodProductionQuantity
        {
            get
            {
                return this.periodProductionQuantityField;
            }
            set
            {
                this.periodProductionQuantityField = value;
            }
        }

        public AAAPeriodType SpecifiedAAAPeriod
        {
            get
            {
                return this.specifiedAAAPeriodField;
            }
            set
            {
                this.specifiedAAAPeriodField = value;
            }
        }
    }
}