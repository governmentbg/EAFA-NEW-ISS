namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOHSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOHSupplyChainTradeTransactionType
    {

        private CIOHSupplyChainTradeAgreementType applicableCIOHSupplyChainTradeAgreementField;

        private CIOHSupplyChainTradeDeliveryType applicableCIOHSupplyChainTradeDeliveryField;

        private CIOHSupplyChainTradeSettlementType applicableCIOHSupplyChainTradeSettlementField;

        private CIOLSupplyChainTradeLineItemType[] includedCIOLSupplyChainTradeLineItemField;

        public CIOHSupplyChainTradeAgreementType ApplicableCIOHSupplyChainTradeAgreement
        {
            get => this.applicableCIOHSupplyChainTradeAgreementField;
            set => this.applicableCIOHSupplyChainTradeAgreementField = value;
        }

        public CIOHSupplyChainTradeDeliveryType ApplicableCIOHSupplyChainTradeDelivery
        {
            get => this.applicableCIOHSupplyChainTradeDeliveryField;
            set => this.applicableCIOHSupplyChainTradeDeliveryField = value;
        }

        public CIOHSupplyChainTradeSettlementType ApplicableCIOHSupplyChainTradeSettlement
        {
            get => this.applicableCIOHSupplyChainTradeSettlementField;
            set => this.applicableCIOHSupplyChainTradeSettlementField = value;
        }

        [XmlElement("IncludedCIOLSupplyChainTradeLineItem")]
        public CIOLSupplyChainTradeLineItemType[] IncludedCIOLSupplyChainTradeLineItem
        {
            get => this.includedCIOLSupplyChainTradeLineItemField;
            set => this.includedCIOLSupplyChainTradeLineItemField = value;
        }
    }
}