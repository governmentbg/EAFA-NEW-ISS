namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradePaymentDiscountTerms", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradePaymentDiscountTermsType
    {

        private DateTimeType basisDateTimeField;

        private MeasureType basisPeriodMeasureField;

        private AmountType basisAmountField;

        private PercentType calculationPercentField;

        private AmountType actualDiscountAmountField;

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

        public AmountType ActualDiscountAmount
        {
            get
            {
                return this.actualDiscountAmountField;
            }
            set
            {
                this.actualDiscountAmountField = value;
            }
        }
    }
}