namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalBookTax", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalBookTaxType
    {

        private IDType idField;

        private AmountType calculatedAmountField;

        private TaxTypeCodeType typeCodeField;

        private TextType exemptionReasonField;

        private RateType calculatedRateField;

        private NumericType calculationSequenceNumericField;

        private QuantityType basisQuantityField;

        private AmountType basisAmountField;

        private TaxCategoryCodeType categoryCodeField;

        private CurrencyCodeType currencyCodeField;

        private TextType jurisdictionField;

        private IndicatorType customsDutyIndicatorField;

        private TaxExemptionReasonCodeType[] exemptionReasonCodeField;

        private RateType taxBasisAllowanceRateField;

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

        public AmountType CalculatedAmount
        {
            get
            {
                return this.calculatedAmountField;
            }
            set
            {
                this.calculatedAmountField = value;
            }
        }

        public TaxTypeCodeType TypeCode
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

        public TextType ExemptionReason
        {
            get
            {
                return this.exemptionReasonField;
            }
            set
            {
                this.exemptionReasonField = value;
            }
        }

        public RateType CalculatedRate
        {
            get
            {
                return this.calculatedRateField;
            }
            set
            {
                this.calculatedRateField = value;
            }
        }

        public NumericType CalculationSequenceNumeric
        {
            get
            {
                return this.calculationSequenceNumericField;
            }
            set
            {
                this.calculationSequenceNumericField = value;
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

        public TaxCategoryCodeType CategoryCode
        {
            get
            {
                return this.categoryCodeField;
            }
            set
            {
                this.categoryCodeField = value;
            }
        }

        public CurrencyCodeType CurrencyCode
        {
            get
            {
                return this.currencyCodeField;
            }
            set
            {
                this.currencyCodeField = value;
            }
        }

        public TextType Jurisdiction
        {
            get
            {
                return this.jurisdictionField;
            }
            set
            {
                this.jurisdictionField = value;
            }
        }

        public IndicatorType CustomsDutyIndicator
        {
            get
            {
                return this.customsDutyIndicatorField;
            }
            set
            {
                this.customsDutyIndicatorField = value;
            }
        }

        [XmlElement("ExemptionReasonCode")]
        public TaxExemptionReasonCodeType[] ExemptionReasonCode
        {
            get
            {
                return this.exemptionReasonCodeField;
            }
            set
            {
                this.exemptionReasonCodeField = value;
            }
        }

        public RateType TaxBasisAllowanceRate
        {
            get
            {
                return this.taxBasisAllowanceRateField;
            }
            set
            {
                this.taxBasisAllowanceRateField = value;
            }
        }
    }
}