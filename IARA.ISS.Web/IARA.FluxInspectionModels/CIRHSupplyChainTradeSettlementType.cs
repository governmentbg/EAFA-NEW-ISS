namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRHSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRHSupplyChainTradeSettlementType
    {

        private CurrencyCodeType paymentCurrencyCodeField;

        private CITradePartyType payeeCITradePartyField;

        private CITradePartyType payerCITradePartyField;

        private CITradeCurrencyExchangeType paymentApplicableCITradeCurrencyExchangeField;

        private CIRHTradeSettlementMonetarySummationType specifiedCIRHTradeSettlementMonetarySummationField;

        private CITradeSettlementPaymentMeansType specifiedCITradeSettlementPaymentMeansField;

        private CITradeTaxType[] applicableCITradeTaxField;

        private CITradePartyType[] relevantCITradePartyField;

        private CITradePaymentTermsType[] specifiedCITradePaymentTermsField;

        public CurrencyCodeType PaymentCurrencyCode
        {
            get => this.paymentCurrencyCodeField;
            set => this.paymentCurrencyCodeField = value;
        }

        public CITradePartyType PayeeCITradeParty
        {
            get => this.payeeCITradePartyField;
            set => this.payeeCITradePartyField = value;
        }

        public CITradePartyType PayerCITradeParty
        {
            get => this.payerCITradePartyField;
            set => this.payerCITradePartyField = value;
        }

        public CITradeCurrencyExchangeType PaymentApplicableCITradeCurrencyExchange
        {
            get => this.paymentApplicableCITradeCurrencyExchangeField;
            set => this.paymentApplicableCITradeCurrencyExchangeField = value;
        }

        public CIRHTradeSettlementMonetarySummationType SpecifiedCIRHTradeSettlementMonetarySummation
        {
            get => this.specifiedCIRHTradeSettlementMonetarySummationField;
            set => this.specifiedCIRHTradeSettlementMonetarySummationField = value;
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

        [XmlElement("RelevantCITradeParty")]
        public CITradePartyType[] RelevantCITradeParty
        {
            get => this.relevantCITradePartyField;
            set => this.relevantCITradePartyField = value;
        }

        [XmlElement("SpecifiedCITradePaymentTerms")]
        public CITradePaymentTermsType[] SpecifiedCITradePaymentTerms
        {
            get => this.specifiedCITradePaymentTermsField;
            set => this.specifiedCITradePaymentTermsField = value;
        }
    }
}