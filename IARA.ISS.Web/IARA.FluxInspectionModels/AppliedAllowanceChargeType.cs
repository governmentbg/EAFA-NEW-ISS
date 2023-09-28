namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AppliedAllowanceCharge", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AppliedAllowanceChargeType
    {

        private AmountType actualAmountField;

        private TextType descriptionField;

        private CodeType reasonCodeField;

        private PercentType calculationPercentField;

        private AmountType basisAmountField;

        private IndicatorType chargeIndicatorField;

        private AppliedTaxType categoryAppliedTaxField;

        public AmountType ActualAmount
        {
            get
            {
                return this.actualAmountField;
            }
            set
            {
                this.actualAmountField = value;
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

        public CodeType ReasonCode
        {
            get
            {
                return this.reasonCodeField;
            }
            set
            {
                this.reasonCodeField = value;
            }
        }

        public PercentType CalculationPercent
        {
            get
            {
                return this.calculationPercentField;
            }
            set
            {
                this.calculationPercentField = value;
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

        public IndicatorType ChargeIndicator
        {
            get
            {
                return this.chargeIndicatorField;
            }
            set
            {
                this.chargeIndicatorField = value;
            }
        }

        public AppliedTaxType CategoryAppliedTax
        {
            get
            {
                return this.categoryAppliedTaxField;
            }
            set
            {
                this.categoryAppliedTaxField = value;
            }
        }
    }
}