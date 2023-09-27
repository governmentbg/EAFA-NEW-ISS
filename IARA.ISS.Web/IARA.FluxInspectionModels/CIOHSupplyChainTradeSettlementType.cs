namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOHSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOHSupplyChainTradeSettlementType
    {

        private CurrencyCodeType taxCurrencyCodeField;

        private CurrencyCodeType orderCurrencyCodeField;

        private CurrencyCodeType invoiceCurrencyCodeField;

        private CurrencyCodeType priceCurrencyCodeField;

        private AmountType[] duePayableAmountField;

        private CITradePartyType invoiceeCITradePartyField;

        private CITradePartyType payerCITradePartyField;

        private CITradeCurrencyExchangeType taxApplicableCITradeCurrencyExchangeField;

        private CITradeCurrencyExchangeType orderApplicableCITradeCurrencyExchangeField;

        private CITradeCurrencyExchangeType invoiceApplicableCITradeCurrencyExchangeField;

        private CITradeCurrencyExchangeType priceApplicableCITradeCurrencyExchangeField;

        private CITradeSettlementPaymentMeansType specifiedCITradeSettlementPaymentMeansField;

        private CITradeTaxType[] applicableCITradeTaxField;

        private CITradeAllowanceChargeType[] specifiedCITradeAllowanceChargeField;

        private CITradePaymentTermsType specifiedCITradePaymentTermsField;

        private CIOHTradeSettlementMonetarySummationType specifiedCIOHTradeSettlementMonetarySummationField;

        private CITradeAccountingAccountType[] payableSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType[] purchaseSpecifiedCITradeAccountingAccountField;

        public CurrencyCodeType TaxCurrencyCode
        {
            get => this.taxCurrencyCodeField;
            set => this.taxCurrencyCodeField = value;
        }

        public CurrencyCodeType OrderCurrencyCode
        {
            get => this.orderCurrencyCodeField;
            set => this.orderCurrencyCodeField = value;
        }

        public CurrencyCodeType InvoiceCurrencyCode
        {
            get => this.invoiceCurrencyCodeField;
            set => this.invoiceCurrencyCodeField = value;
        }

        public CurrencyCodeType PriceCurrencyCode
        {
            get => this.priceCurrencyCodeField;
            set => this.priceCurrencyCodeField = value;
        }

        [XmlElement("DuePayableAmount")]
        public AmountType[] DuePayableAmount
        {
            get => this.duePayableAmountField;
            set => this.duePayableAmountField = value;
        }

        public CITradePartyType InvoiceeCITradeParty
        {
            get => this.invoiceeCITradePartyField;
            set => this.invoiceeCITradePartyField = value;
        }

        public CITradePartyType PayerCITradeParty
        {
            get => this.payerCITradePartyField;
            set => this.payerCITradePartyField = value;
        }

        public CITradeCurrencyExchangeType TaxApplicableCITradeCurrencyExchange
        {
            get => this.taxApplicableCITradeCurrencyExchangeField;
            set => this.taxApplicableCITradeCurrencyExchangeField = value;
        }

        public CITradeCurrencyExchangeType OrderApplicableCITradeCurrencyExchange
        {
            get => this.orderApplicableCITradeCurrencyExchangeField;
            set => this.orderApplicableCITradeCurrencyExchangeField = value;
        }

        public CITradeCurrencyExchangeType InvoiceApplicableCITradeCurrencyExchange
        {
            get => this.invoiceApplicableCITradeCurrencyExchangeField;
            set => this.invoiceApplicableCITradeCurrencyExchangeField = value;
        }

        public CITradeCurrencyExchangeType PriceApplicableCITradeCurrencyExchange
        {
            get => this.priceApplicableCITradeCurrencyExchangeField;
            set => this.priceApplicableCITradeCurrencyExchangeField = value;
        }

        public CITradeSettlementPaymentMeansType SpecifiedCITradeSettlementPaymentMeans
        {
            get => this.specifiedCITradeSettlementPaymentMeansField;
            set => this.specifiedCITradeSettlementPaymentMeansField = value;
        }

        [XmlElement("ApplicableCITradeTax")]
        public CITradeTaxType[] ApplicableCITradeTax
        {
            get => this.applicableCITradeTaxField;
            set => this.applicableCITradeTaxField = value;
        }

        [XmlElement("SpecifiedCITradeAllowanceCharge")]
        public CITradeAllowanceChargeType[] SpecifiedCITradeAllowanceCharge
        {
            get => this.specifiedCITradeAllowanceChargeField;
            set => this.specifiedCITradeAllowanceChargeField = value;
        }

        public CITradePaymentTermsType SpecifiedCITradePaymentTerms
        {
            get => this.specifiedCITradePaymentTermsField;
            set => this.specifiedCITradePaymentTermsField = value;
        }

        public CIOHTradeSettlementMonetarySummationType SpecifiedCIOHTradeSettlementMonetarySummation
        {
            get => this.specifiedCIOHTradeSettlementMonetarySummationField;
            set => this.specifiedCIOHTradeSettlementMonetarySummationField = value;
        }

        [XmlElement("PayableSpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] PayableSpecifiedCITradeAccountingAccount
        {
            get => this.payableSpecifiedCITradeAccountingAccountField;
            set => this.payableSpecifiedCITradeAccountingAccountField = value;
        }

        [XmlElement("PurchaseSpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] PurchaseSpecifiedCITradeAccountingAccount
        {
            get => this.purchaseSpecifiedCITradeAccountingAccountField;
            set => this.purchaseSpecifiedCITradeAccountingAccountField = value;
        }
    }
}