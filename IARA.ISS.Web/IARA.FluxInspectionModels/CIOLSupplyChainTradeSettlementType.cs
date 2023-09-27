namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOLSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOLSupplyChainTradeSettlementType
    {

        private CITradePartyType invoiceeCITradePartyField;

        private CITradePartyType payerCITradePartyField;

        private CITradeTaxType[] applicableCITradeTaxField;

        private CITradeAllowanceChargeType[] specifiedCITradeAllowanceChargeField;

        private CIOLTradeSettlementMonetarySummationType specifiedCIOLTradeSettlementMonetarySummationField;

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

        public CIOLTradeSettlementMonetarySummationType SpecifiedCIOLTradeSettlementMonetarySummation
        {
            get => this.specifiedCIOLTradeSettlementMonetarySummationField;
            set => this.specifiedCIOLTradeSettlementMonetarySummationField = value;
        }
    }
}