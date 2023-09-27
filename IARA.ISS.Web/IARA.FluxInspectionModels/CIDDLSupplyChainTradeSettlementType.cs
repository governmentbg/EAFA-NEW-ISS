namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDLSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDLSupplyChainTradeSettlementType
    {

        private CITradeAccountingAccountType[] payableSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType[] purchaseSpecifiedCITradeAccountingAccountField;

        private CITradeTaxType applicableCITradeTaxField;

        private CIDDLTradeSettlementMonetarySummationType specifiedCIDDLTradeSettlementMonetarySummationField;

        [XmlElement("PayableSpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] PayableSpecifiedCITradeAccountingAccount
        {
            get
            {
                return this.payableSpecifiedCITradeAccountingAccountField;
            }
            set
            {
                this.payableSpecifiedCITradeAccountingAccountField = value;
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

        public CITradeTaxType ApplicableCITradeTax
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

        public CIDDLTradeSettlementMonetarySummationType SpecifiedCIDDLTradeSettlementMonetarySummation
        {
            get
            {
                return this.specifiedCIDDLTradeSettlementMonetarySummationField;
            }
            set
            {
                this.specifiedCIDDLTradeSettlementMonetarySummationField = value;
            }
        }
    }
}