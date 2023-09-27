namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAATrialBalanceAccountingLineMonetaryValue", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAATrialBalanceAccountingLineMonetaryValueType
    {

        private AmountType localAccountingCurrencyAmountField;

        private AmountType alternateCurrencyAmountField;

        private CurrencyCodeType alternateCurrencyAmountTypeCodeField;

        private AccountingDebitCreditStatusCodeType debitCreditCodeField;

        private AccountingAmountQualifierCodeType amountQualifierCodeField;

        private QuantityType balanceQuantityField;

        private AAAPeriodType specifiedAAAPeriodField;

        public AmountType LocalAccountingCurrencyAmount
        {
            get
            {
                return this.localAccountingCurrencyAmountField;
            }
            set
            {
                this.localAccountingCurrencyAmountField = value;
            }
        }

        public AmountType AlternateCurrencyAmount
        {
            get
            {
                return this.alternateCurrencyAmountField;
            }
            set
            {
                this.alternateCurrencyAmountField = value;
            }
        }

        public CurrencyCodeType AlternateCurrencyAmountTypeCode
        {
            get
            {
                return this.alternateCurrencyAmountTypeCodeField;
            }
            set
            {
                this.alternateCurrencyAmountTypeCodeField = value;
            }
        }

        public AccountingDebitCreditStatusCodeType DebitCreditCode
        {
            get
            {
                return this.debitCreditCodeField;
            }
            set
            {
                this.debitCreditCodeField = value;
            }
        }

        public AccountingAmountQualifierCodeType AmountQualifierCode
        {
            get
            {
                return this.amountQualifierCodeField;
            }
            set
            {
                this.amountQualifierCodeField = value;
            }
        }

        public QuantityType BalanceQuantity
        {
            get
            {
                return this.balanceQuantityField;
            }
            set
            {
                this.balanceQuantityField = value;
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