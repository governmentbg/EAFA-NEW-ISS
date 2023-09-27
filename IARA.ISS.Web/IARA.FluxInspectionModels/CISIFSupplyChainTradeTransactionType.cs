namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISIFSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISIFSupplyChainTradeTransactionType
    {

        private CISSupplyChainTradeAgreementType applicableCISSupplyChainTradeAgreementField;

        private CISHSupplyChainTradeDeliveryType applicableCISHSupplyChainTradeDeliveryField;

        private CISIFLSupplyChainTradeLineItemType[] includedCISIFLSupplyChainTradeLineItemField;

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

        [XmlElement("IncludedCISIFLSupplyChainTradeLineItem")]
        public CISIFLSupplyChainTradeLineItemType[] IncludedCISIFLSupplyChainTradeLineItem
        {
            get => this.includedCISIFLSupplyChainTradeLineItemField;
            set => this.includedCISIFLSupplyChainTradeLineItemField = value;
        }
    }
}