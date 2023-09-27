namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQLSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQLSupplyChainTradeSettlementType
    {

        private CITradeAllowanceChargeType[] specifiedCITradeAllowanceChargeField;

        private CIQLTradeSettlementMonetarySummationType specifiedCIQLTradeSettlementMonetarySummationField;

        private CITradeTaxType[] applicableCITradeTaxField;

        [XmlElement("SpecifiedCITradeAllowanceCharge")]
        public CITradeAllowanceChargeType[] SpecifiedCITradeAllowanceCharge
        {
            get => this.specifiedCITradeAllowanceChargeField;
            set => this.specifiedCITradeAllowanceChargeField = value;
        }

        public CIQLTradeSettlementMonetarySummationType SpecifiedCIQLTradeSettlementMonetarySummation
        {
            get => this.specifiedCIQLTradeSettlementMonetarySummationField;
            set => this.specifiedCIQLTradeSettlementMonetarySummationField = value;
        }

        [XmlElement("ApplicableCITradeTax")]
        public CITradeTaxType[] ApplicableCITradeTax
        {
            get => this.applicableCITradeTaxField;
            set => this.applicableCITradeTaxField = value;
        }
    }
}