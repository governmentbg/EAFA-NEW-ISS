namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISSISupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISSISupplyChainTradeTransactionType
    {

        private CISSupplyChainTradeAgreementType applicableCISSupplyChainTradeAgreementField;

        private CISHSupplyChainTradeDeliveryType applicableCISHSupplyChainTradeDeliveryField;

        private CISSILSupplyChainTradeLineItemType[] includedCISSILSupplyChainTradeLineItemField;

        private CISSupplyChainTradeSettlementType applicableCISSupplyChainTradeSettlementField;

        public CISSupplyChainTradeAgreementType ApplicableCISSupplyChainTradeAgreement
        {
            get => this.applicableCISSupplyChainTradeAgreementField;
            set => this.applicableCISSupplyChainTradeAgreementField = value;
        }

        public CISHSupplyChainTradeDeliveryType ApplicableCISHSupplyChainTradeDelivery
        {
            get => this.applicableCISHSupplyChainTradeDeliveryField;
            set => this.applicableCISHSupplyChainTradeDeliveryField = value;
        }

        [XmlElement("IncludedCISSILSupplyChainTradeLineItem")]
        public CISSILSupplyChainTradeLineItemType[] IncludedCISSILSupplyChainTradeLineItem
        {
            get => this.includedCISSILSupplyChainTradeLineItemField;
            set => this.includedCISSILSupplyChainTradeLineItemField = value;
        }

        public CISSupplyChainTradeSettlementType ApplicableCISSupplyChainTradeSettlement
        {
            get => this.applicableCISSupplyChainTradeSettlementField;
            set => this.applicableCISSupplyChainTradeSettlementField = value;
        }
    }
}