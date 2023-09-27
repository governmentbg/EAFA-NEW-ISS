namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISSNSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISSNSupplyChainTradeTransactionType
    {

        private CISSupplyChainTradeAgreementType applicableCISSupplyChainTradeAgreementField;

        private CISHSupplyChainTradeDeliveryType applicableCISHSupplyChainTradeDeliveryField;

        private CISSNLSupplyChainTradeLineItemType[] includedCISSNLSupplyChainTradeLineItemField;

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

        [XmlElement("IncludedCISSNLSupplyChainTradeLineItem")]
        public CISSNLSupplyChainTradeLineItemType[] IncludedCISSNLSupplyChainTradeLineItem
        {
            get => this.includedCISSNLSupplyChainTradeLineItemField;
            set => this.includedCISSNLSupplyChainTradeLineItemField = value;
        }
    }
}