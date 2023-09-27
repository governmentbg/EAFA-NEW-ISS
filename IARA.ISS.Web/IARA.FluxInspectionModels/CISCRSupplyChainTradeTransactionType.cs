namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISCRSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISCRSupplyChainTradeTransactionType
    {

        private IDType[] idField;

        private IDType salesAgentAssignedIDField;

        private CodeType typeCodeField;

        private TextType[] informationField;

        private QuantityType lineItemQuantityField;

        private CISSupplyChainTradeAgreementType applicableCISSupplyChainTradeAgreementField;

        private CISHSupplyChainTradeDeliveryType applicableCISHSupplyChainTradeDeliveryField;

        private CISHSupplyChainTradeSettlementType applicableCISHSupplyChainTradeSettlementField;

        private CISCRLSupplyChainTradeLineItemType[] includedCISCRLSupplyChainTradeLineItemField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType SalesAgentAssignedID
        {
            get => this.salesAgentAssignedIDField;
            set => this.salesAgentAssignedIDField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("Information")]
        public TextType[] Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public QuantityType LineItemQuantity
        {
            get => this.lineItemQuantityField;
            set => this.lineItemQuantityField = value;
        }

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

        public CISHSupplyChainTradeSettlementType ApplicableCISHSupplyChainTradeSettlement
        {
            get => this.applicableCISHSupplyChainTradeSettlementField;
            set => this.applicableCISHSupplyChainTradeSettlementField = value;
        }

        [XmlElement("IncludedCISCRLSupplyChainTradeLineItem")]
        public CISCRLSupplyChainTradeLineItemType[] IncludedCISCRLSupplyChainTradeLineItem
        {
            get => this.includedCISCRLSupplyChainTradeLineItemField;
            set => this.includedCISCRLSupplyChainTradeLineItemField = value;
        }
    }
}