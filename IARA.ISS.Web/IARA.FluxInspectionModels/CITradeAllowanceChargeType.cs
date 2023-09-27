namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeAllowanceCharge", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeAllowanceChargeType
    {

        private IndicatorType chargeIndicatorField;

        private IDType idField;

        private NumericType sequenceNumericField;

        private PercentType calculationPercentField;

        private QuantityType basisQuantityField;

        private IndicatorType prepaidIndicatorField;

        private AmountType[] actualAmountField;

        private AllowanceChargeReasonCodeType reasonCodeField;

        private TextType reasonField;

        private AllowanceChargeIdentificationCodeType typeCodeField;

        private AmountType basisAmountField;

        private AmountType unitBasisAmountField;

        private CITradeTaxType[] categoryCITradeTaxField;

        private CITradeCurrencyExchangeType actualCITradeCurrencyExchangeField;

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

        public NumericType SequenceNumeric
        {
            get
            {
                return this.sequenceNumericField;
            }
            set
            {
                this.sequenceNumericField = value;
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

        public QuantityType BasisQuantity
        {
            get
            {
                return this.basisQuantityField;
            }
            set
            {
                this.basisQuantityField = value;
            }
        }

        public IndicatorType PrepaidIndicator
        {
            get
            {
                return this.prepaidIndicatorField;
            }
            set
            {
                this.prepaidIndicatorField = value;
            }
        }

        [XmlElement("ActualAmount")]
        public AmountType[] ActualAmount
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

        public AllowanceChargeReasonCodeType ReasonCode
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

        public TextType Reason
        {
            get
            {
                return this.reasonField;
            }
            set
            {
                this.reasonField = value;
            }
        }

        public AllowanceChargeIdentificationCodeType TypeCode
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

        public AmountType UnitBasisAmount
        {
            get
            {
                return this.unitBasisAmountField;
            }
            set
            {
                this.unitBasisAmountField = value;
            }
        }

        [XmlElement("CategoryCITradeTax")]
        public CITradeTaxType[] CategoryCITradeTax
        {
            get
            {
                return this.categoryCITradeTaxField;
            }
            set
            {
                this.categoryCITradeTaxField = value;
            }
        }

        public CITradeCurrencyExchangeType ActualCITradeCurrencyExchange
        {
            get
            {
                return this.actualCITradeCurrencyExchangeField;
            }
            set
            {
                this.actualCITradeCurrencyExchangeField = value;
            }
        }
    }
}