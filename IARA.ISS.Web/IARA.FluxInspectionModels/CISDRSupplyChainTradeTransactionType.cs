namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISDRSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISDRSupplyChainTradeTransactionType
    {

        private CISSupplyChainTradeAgreementType applicableCISSupplyChainTradeAgreementField;

        private CISHSupplyChainTradeDeliveryType applicableCISHSupplyChainTradeDeliveryField;

        private CISDRLSupplyChainTradeLineItemType[] includedCISDRLSupplyChainTradeLineItemField;

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

        [XmlElement("IncludedCISDRLSupplyChainTradeLineItem")]
        public CISDRLSupplyChainTradeLineItemType[] IncludedCISDRLSupplyChainTradeLineItem
        {
            get => this.includedCISDRLSupplyChainTradeLineItemField;
            set => this.includedCISDRLSupplyChainTradeLineItemField = value;
        }
    }
}