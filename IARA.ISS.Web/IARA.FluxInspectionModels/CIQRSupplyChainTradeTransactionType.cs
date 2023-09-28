namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQRSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQRSupplyChainTradeTransactionType
    {

        private CIQHSupplyChainTradeAgreementType applicableCIQHSupplyChainTradeAgreementField;

        private CIQHSupplyChainTradeDeliveryType applicableCIQHSupplyChainTradeDeliveryField;

        private CIQRLSupplyChainTradeLineItemType[] includedCIQRLSupplyChainTradeLineItemField;

        private CIQHSupplyChainTradeSettlementType applicableCIQHSupplyChainTradeSettlementField;

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

        [XmlElement("IncludedCIQRLSupplyChainTradeLineItem")]
        public CIQRLSupplyChainTradeLineItemType[] IncludedCIQRLSupplyChainTradeLineItem
        {
            get => this.includedCIQRLSupplyChainTradeLineItemField;
            set => this.includedCIQRLSupplyChainTradeLineItemField = value;
        }

        public CIQHSupplyChainTradeSettlementType ApplicableCIQHSupplyChainTradeSettlement
        {
            get => this.applicableCIQHSupplyChainTradeSettlementField;
            set => this.applicableCIQHSupplyChainTradeSettlementField = value;
        }
    }
}