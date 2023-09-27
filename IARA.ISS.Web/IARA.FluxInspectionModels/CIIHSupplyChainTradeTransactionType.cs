namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIIHSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIIHSupplyChainTradeTransactionType
    {

        private CIIHSupplyChainTradeAgreementType applicableCIIHSupplyChainTradeAgreementField;

        private CIIHSupplyChainTradeDeliveryType applicableCIIHSupplyChainTradeDeliveryField;

        private CIIHSupplyChainTradeSettlementType applicableCIIHSupplyChainTradeSettlementField;

        private CIILSupplyChainTradeLineItemType[] includedCIILSupplyChainTradeLineItemField;

        public CIIHSupplyChainTradeAgreementType ApplicableCIIHSupplyChainTradeAgreement
        {
            get => this.applicableCIIHSupplyChainTradeAgreementField;
            set => this.applicableCIIHSupplyChainTradeAgreementField = value;
        }

        public CIIHSupplyChainTradeDeliveryType ApplicableCIIHSupplyChainTradeDelivery
        {
            get => this.applicableCIIHSupplyChainTradeDeliveryField;
            set => this.applicableCIIHSupplyChainTradeDeliveryField = value;
        }

        public CIIHSupplyChainTradeSettlementType ApplicableCIIHSupplyChainTradeSettlement
        {
            get => this.applicableCIIHSupplyChainTradeSettlementField;
            set => this.applicableCIIHSupplyChainTradeSettlementField = value;
        }

        [XmlElement("IncludedCIILSupplyChainTradeLineItem")]
        public CIILSupplyChainTradeLineItemType[] IncludedCIILSupplyChainTradeLineItem
        {
            get => this.includedCIILSupplyChainTradeLineItemField;
            set => this.includedCIILSupplyChainTradeLineItemField = value;
        }
    }
}