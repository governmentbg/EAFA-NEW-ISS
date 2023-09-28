namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDHSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDHSupplyChainTradeSettlementType
    {

        private CodeType priceCurrencyCodeField;

        private CITradeAccountingAccountType[] purchaseSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType[] salesSpecifiedCITradeAccountingAccountField;

        private CITradeTaxType[] applicableCITradeTaxField;

        private CITradePaymentTermsType specifiedCITradePaymentTermsField;

        private CIDDHTradeSettlementMonetarySummationType specifiedCIDDHTradeSettlementMonetarySummationField;

        public CodeType PriceCurrencyCode
        {
            get
            {
                return this.priceCurrencyCodeField;
            }
            set
            {
                this.priceCurrencyCodeField = value;
            }
        }

        [XmlElement("PurchaseSpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] PurchaseSpecifiedCITradeAccountingAccount
        {
            get
            {
                return this.purchaseSpecifiedCITradeAccountingAccountField;
            }
            set
            {
                this.purchaseSpecifiedCITradeAccountingAccountField = value;
            }
        }

        [XmlElement("SalesSpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] SalesSpecifiedCITradeAccountingAccount
        {
            get
            {
                return this.salesSpecifiedCITradeAccountingAccountField;
            }
            set
            {
                this.salesSpecifiedCITradeAccountingAccountField = value;
            }
        }

        [XmlElement("ApplicableCITradeTax")]
        public CITradeTaxType[] ApplicableCITradeTax
        {
            get
            {
                return this.applicableCITradeTaxField;
            }
            set
            {
                this.applicableCITradeTaxField = value;
            }
        }

        public CITradePaymentTermsType SpecifiedCITradePaymentTerms
        {
            get
            {
                return this.specifiedCITradePaymentTermsField;
            }
            set
            {
                this.specifiedCITradePaymentTermsField = value;
            }
        }

        public CIDDHTradeSettlementMonetarySummationType SpecifiedCIDDHTradeSettlementMonetarySummation
        {
            get
            {
                return this.specifiedCIDDHTradeSettlementMonetarySummationField;
            }
            set
            {
                this.specifiedCIDDHTradeSettlementMonetarySummationField = value;
            }
        }
    }
}