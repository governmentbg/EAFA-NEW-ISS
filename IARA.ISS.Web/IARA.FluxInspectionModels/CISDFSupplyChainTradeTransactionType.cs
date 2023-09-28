namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISDFSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISDFSupplyChainTradeTransactionType
    {

        private CISSupplyChainTradeAgreementType applicableCISSupplyChainTradeAgreementField;

        private CISHSupplyChainTradeDeliveryType applicableCISHSupplyChainTradeDeliveryField;

        private CISDFLSupplyChainTradeLineItemType[] includedCISDFLSupplyChainTradeLineItemField;

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

        [XmlElement("IncludedCISDFLSupplyChainTradeLineItem")]
        public CISDFLSupplyChainTradeLineItemType[] IncludedCISDFLSupplyChainTradeLineItem
        {
            get => this.includedCISDFLSupplyChainTradeLineItemField;
            set => this.includedCISDFLSupplyChainTradeLineItemField = value;
        }
    }
}