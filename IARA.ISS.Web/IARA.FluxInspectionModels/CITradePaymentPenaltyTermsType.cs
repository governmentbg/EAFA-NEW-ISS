namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradePaymentPenaltyTerms", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradePaymentPenaltyTermsType
    {

        private DateTimeType basisDateTimeField;

        private MeasureType basisPeriodMeasureField;

        private AmountType basisAmountField;

        private PercentType calculationPercentField;

        private AmountType actualPenaltyAmountField;

        public DateTimeType BasisDateTime
        {
            get
            {
                return this.basisDateTimeField;
            }
            set
            {
                this.basisDateTimeField = value;
            }
        }

        public MeasureType BasisPeriodMeasure
        {
            get
            {
                return this.basisPeriodMeasureField;
            }
            set
            {
                this.basisPeriodMeasureField = value;
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

        public AmountType ActualPenaltyAmount
        {
            get
            {
                return this.actualPenaltyAmountField;
            }
            set
            {
                this.actualPenaltyAmountField = value;
            }
        }
    }
}