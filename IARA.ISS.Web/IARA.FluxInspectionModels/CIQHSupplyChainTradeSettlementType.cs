namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQHSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQHSupplyChainTradeSettlementType
    {

        private CurrencyCodeType priceCurrencyCodeField;

        private CurrencyCodeType quotationCurrencyCodeField;

        private CITradePartyType invoiceeCITradePartyField;

        private CITradePartyType payerCITradePartyField;

        private CITradeAllowanceChargeType[] specifiedCITradeAllowanceChargeField;

        private CITradeCurrencyExchangeType priceApplicableCITradeCurrencyExchangeField;

        private CITradeCurrencyExchangeType quotationApplicableCITradeCurrencyExchangeField;

        private CITradeCurrencyExchangeType[] taxApplicableCITradeCurrencyExchangeField;

        private CIQHTradeSettlementMonetarySummationType specifiedCIQHTradeSettlementMonetarySummationField;

        private CITradeSettlementPaymentMeansType[] specifiedCITradeSettlementPaymentMeansField;

        private CITradePaymentTermsType specifiedCITradePaymentTermsField;

        private CITradeTaxType[] applicableCITradeTaxField;

        public CurrencyCodeType PriceCurrencyCode
        {
            get => this.priceCurrencyCodeField;
            set => this.priceCurrencyCodeField = value;
        }

        public CurrencyCodeType QuotationCurrencyCode
        {
            get => this.quotationCurrencyCodeField;
            set => this.quotationCurrencyCodeField = value;
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

        [XmlElement("SpecifiedCITradeAllowanceCharge")]
        public CITradeAllowanceChargeType[] SpecifiedCITradeAllowanceCharge
        {
            get => this.specifiedCITradeAllowanceChargeField;
            set => this.specifiedCITradeAllowanceChargeField = value;
        }

        public CITradeCurrencyExchangeType PriceApplicableCITradeCurrencyExchange
        {
            get => this.priceApplicableCITradeCurrencyExchangeField;
            set => this.priceApplicableCITradeCurrencyExchangeField = value;
        }

        public CITradeCurrencyExchangeType QuotationApplicableCITradeCurrencyExchange
        {
            get => this.quotationApplicableCITradeCurrencyExchangeField;
            set => this.quotationApplicableCITradeCurrencyExchangeField = value;
        }

        [XmlElement("TaxApplicableCITradeCurrencyExchange")]
        public CITradeCurrencyExchangeType[] TaxApplicableCITradeCurrencyExchange
        {
            get => this.taxApplicableCITradeCurrencyExchangeField;
            set => this.taxApplicableCITradeCurrencyExchangeField = value;
        }

        public CIQHTradeSettlementMonetarySummationType SpecifiedCIQHTradeSettlementMonetarySummation
        {
            get => this.specifiedCIQHTradeSettlementMonetarySummationField;
            set => this.specifiedCIQHTradeSettlementMonetarySummationField = value;
        }

        [XmlElement("SpecifiedCITradeSettlementPaymentMeans")]
        public CITradeSettlementPaymentMeansType[] SpecifiedCITradeSettlementPaymentMeans
        {
            get => this.specifiedCITradeSettlementPaymentMeansField;
            set => this.specifiedCITradeSettlementPaymentMeansField = value;
        }

        public CITradePaymentTermsType SpecifiedCITradePaymentTerms
        {
            get => this.specifiedCITradePaymentTermsField;
            set => this.specifiedCITradePaymentTermsField = value;
        }

        [XmlElement("ApplicableCITradeTax")]
        public CITradeTaxType[] ApplicableCITradeTax
        {
            get => this.applicableCITradeTaxField;
            set => this.applicableCITradeTaxField = value;
        }
    }
}