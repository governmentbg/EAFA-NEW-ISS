namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportExpectedInformation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportExpectedInformationType
    {

        private IDType responseIndexIDField;

        private AmountType specifiedAmountField;

        private AmountWeightTypeCodeType amountWeightCodeField;

        private NumericType amountDecimalDigitNumericField;

        private IDType referenceIDField;

        private TextType commentField;

        private DateType specifiedDateField;

        private TimeType specifiedTimeField;

        private QuantityType specifiedQuantityField;

        private PercentType specifiedPercentField;

        private IndicatorType responseIndicatorField;

        private CodeType currencyCodeField;

        private NumericType currencyDecimalDigitNumericField;

        private CodeType currencyUsageCodeField;

        public IDType ResponseIndexID
        {
            get
            {
                return this.responseIndexIDField;
            }
            set
            {
                this.responseIndexIDField = value;
            }
        }

        public AmountType SpecifiedAmount
        {
            get
            {
                return this.specifiedAmountField;
            }
            set
            {
                this.specifiedAmountField = value;
            }
        }

        public AmountWeightTypeCodeType AmountWeightCode
        {
            get
            {
                return this.amountWeightCodeField;
            }
            set
            {
                this.amountWeightCodeField = value;
            }
        }

        public NumericType AmountDecimalDigitNumeric
        {
            get
            {
                return this.amountDecimalDigitNumericField;
            }
            set
            {
                this.amountDecimalDigitNumericField = value;
            }
        }

        public IDType ReferenceID
        {
            get
            {
                return this.referenceIDField;
            }
            set
            {
                this.referenceIDField = value;
            }
        }

        public TextType Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        public DateType SpecifiedDate
        {
            get
            {
                return this.specifiedDateField;
            }
            set
            {
                this.specifiedDateField = value;
            }
        }

        public TimeType SpecifiedTime
        {
            get
            {
                return this.specifiedTimeField;
            }
            set
            {
                this.specifiedTimeField = value;
            }
        }

        public QuantityType SpecifiedQuantity
        {
            get
            {
                return this.specifiedQuantityField;
            }
            set
            {
                this.specifiedQuantityField = value;
            }
        }

        public PercentType SpecifiedPercent
        {
            get
            {
                return this.specifiedPercentField;
            }
            set
            {
                this.specifiedPercentField = value;
            }
        }

        public IndicatorType ResponseIndicator
        {
            get
            {
                return this.responseIndicatorField;
            }
            set
            {
                this.responseIndicatorField = value;
            }
        }

        public CodeType CurrencyCode
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

        public NumericType CurrencyDecimalDigitNumeric
        {
            get
            {
                return this.currencyDecimalDigitNumericField;
            }
            set
            {
                this.currencyDecimalDigitNumericField = value;
            }
        }

        public CodeType CurrencyUsageCode
        {
            get
            {
                return this.currencyUsageCodeField;
            }
            set
            {
                this.currencyUsageCodeField = value;
            }
        }
    }
}