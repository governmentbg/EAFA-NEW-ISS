namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeTax", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeTaxType
    {

        private AmountType[] calculatedAmountField;

        private TaxTypeCodeType typeCodeField;

        private TextType exemptionReasonField;

        private RateType calculatedRateField;

        private NumericType calculationSequenceNumericField;

        private QuantityType basisQuantityField;

        private AmountType[] basisAmountField;

        private AmountType[] unitBasisAmountField;

        private TaxCategoryCodeType categoryCodeField;

        private CurrencyCodeType currencyCodeField;

        private TextType[] jurisdictionField;

        private IndicatorType customsDutyIndicatorField;

        private CodeType exemptionReasonCodeField;

        private RateType taxBasisAllowanceRateField;

        private DateType taxPointDateField;

        private TextType typeField;

        private AmountType[] informationAmountField;

        private TextType[] categoryNameField;

        private TimeReferenceCodeType dueDateTypeCodeField;

        private PercentType rateApplicablePercentField;

        private AmountType grandTotalAmountField;

        private CodeType calculationMethodCodeField;

        private IDType localTaxSystemIDField;

        private CITradeAccountingAccountType[] specifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType buyerDeductibleTaxSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType buyerNonDeductibleTaxSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType buyerRepayableTaxSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType sellerPayableTaxSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType sellerRefundableTaxSpecifiedCITradeAccountingAccountField;

        private CITradeCountryType serviceSupplyCITradeCountryField;

        private CITradeLocationType[] placeApplicableCITradeLocationField;

        [XmlElement("CalculatedAmount")]
        public AmountType[] CalculatedAmount
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

        [XmlElement("BasisAmount")]
        public AmountType[] BasisAmount
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

        [XmlElement("UnitBasisAmount")]
        public AmountType[] UnitBasisAmount
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

        [XmlElement("Jurisdiction")]
        public TextType[] Jurisdiction
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

        public CodeType ExemptionReasonCode
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

        public DateType TaxPointDate
        {
            get
            {
                return this.taxPointDateField;
            }
            set
            {
                this.taxPointDateField = value;
            }
        }

        public TextType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        [XmlElement("InformationAmount")]
        public AmountType[] InformationAmount
        {
            get
            {
                return this.informationAmountField;
            }
            set
            {
                this.informationAmountField = value;
            }
        }

        [XmlElement("CategoryName")]
        public TextType[] CategoryName
        {
            get
            {
                return this.categoryNameField;
            }
            set
            {
                this.categoryNameField = value;
            }
        }

        public TimeReferenceCodeType DueDateTypeCode
        {
            get
            {
                return this.dueDateTypeCodeField;
            }
            set
            {
                this.dueDateTypeCodeField = value;
            }
        }

        public PercentType RateApplicablePercent
        {
            get
            {
                return this.rateApplicablePercentField;
            }
            set
            {
                this.rateApplicablePercentField = value;
            }
        }

        public AmountType GrandTotalAmount
        {
            get
            {
                return this.grandTotalAmountField;
            }
            set
            {
                this.grandTotalAmountField = value;
            }
        }

        public CodeType CalculationMethodCode
        {
            get
            {
                return this.calculationMethodCodeField;
            }
            set
            {
                this.calculationMethodCodeField = value;
            }
        }

        public IDType LocalTaxSystemID
        {
            get
            {
                return this.localTaxSystemIDField;
            }
            set
            {
                this.localTaxSystemIDField = value;
            }
        }

        [XmlElement("SpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] SpecifiedCITradeAccountingAccount
        {
            get
            {
                return this.specifiedCITradeAccountingAccountField;
            }
            set
            {
                this.specifiedCITradeAccountingAccountField = value;
            }
        }

        public CITradeAccountingAccountType BuyerDeductibleTaxSpecifiedCITradeAccountingAccount
        {
            get
            {
                return this.buyerDeductibleTaxSpecifiedCITradeAccountingAccountField;
            }
            set
            {
                this.buyerDeductibleTaxSpecifiedCITradeAccountingAccountField = value;
            }
        }

        public CITradeAccountingAccountType BuyerNonDeductibleTaxSpecifiedCITradeAccountingAccount
        {
            get
            {
                return this.buyerNonDeductibleTaxSpecifiedCITradeAccountingAccountField;
            }
            set
            {
                this.buyerNonDeductibleTaxSpecifiedCITradeAccountingAccountField = value;
            }
        }

        public CITradeAccountingAccountType BuyerRepayableTaxSpecifiedCITradeAccountingAccount
        {
            get
            {
                return this.buyerRepayableTaxSpecifiedCITradeAccountingAccountField;
            }
            set
            {
                this.buyerRepayableTaxSpecifiedCITradeAccountingAccountField = value;
            }
        }

        public CITradeAccountingAccountType SellerPayableTaxSpecifiedCITradeAccountingAccount
        {
            get
            {
                return this.sellerPayableTaxSpecifiedCITradeAccountingAccountField;
            }
            set
            {
                this.sellerPayableTaxSpecifiedCITradeAccountingAccountField = value;
            }
        }

        public CITradeAccountingAccountType SellerRefundableTaxSpecifiedCITradeAccountingAccount
        {
            get
            {
                return this.sellerRefundableTaxSpecifiedCITradeAccountingAccountField;
            }
            set
            {
                this.sellerRefundableTaxSpecifiedCITradeAccountingAccountField = value;
            }
        }

        public CITradeCountryType ServiceSupplyCITradeCountry
        {
            get
            {
                return this.serviceSupplyCITradeCountryField;
            }
            set
            {
                this.serviceSupplyCITradeCountryField = value;
            }
        }

        [XmlElement("PlaceApplicableCITradeLocation")]
        public CITradeLocationType[] PlaceApplicableCITradeLocation
        {
            get
            {
                return this.placeApplicableCITradeLocationField;
            }
            set
            {
                this.placeApplicableCITradeLocationField = value;
            }
        }
    }
}