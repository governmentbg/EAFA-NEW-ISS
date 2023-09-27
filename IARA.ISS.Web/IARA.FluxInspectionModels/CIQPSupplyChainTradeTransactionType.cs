namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQPSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQPSupplyChainTradeTransactionType
    {

        private CIQHSupplyChainTradeAgreementType applicableCIQHSupplyChainTradeAgreementField;

        private CIQHSupplyChainTradeDeliveryType applicableCIQHSupplyChainTradeDeliveryField;

        private CIQHSupplyChainTradeSettlementType applicableCIQHSupplyChainTradeSettlementField;

        private CIQPLSupplyChainTradeLineItemType[] includedCIQPLSupplyChainTradeLineItemField;

        public CIQHSupplyChainTradeAgreementType ApplicableCIQHSupplyChainTradeAgreement
        {
            get => this.applicableCIQHSupplyChainTradeAgreementField;
            set => this.applicableCIQHSupplyChainTradeAgreementField = value;
        }

        public CIQHSupplyChainTradeDeliveryType ApplicableCIQHSupplyChainTradeDelivery
        {
            get => this.applicableCIQHSupplyChainTradeDeliveryField;
            set => this.applicableCIQHSupplyChainTradeDeliveryField = value;
        }

        public CIQHSupplyChainTradeSettlementType ApplicableCIQHSupplyChainTradeSettlement
        {
            get => this.applicableCIQHSupplyChainTradeSettlementField;
            set => this.applicableCIQHSupplyChainTradeSettlementField = value;
        }

        [XmlElement("IncludedCIQPLSupplyChainTradeLineItem")]
        public CIQPLSupplyChainTradeLineItemType[] IncludedCIQPLSupplyChainTradeLineItem
        {
            get => this.includedCIQPLSupplyChainTradeLineItemField;
            set => this.includedCIQPLSupplyChainTradeLineItemField = value;
        }
    }
}