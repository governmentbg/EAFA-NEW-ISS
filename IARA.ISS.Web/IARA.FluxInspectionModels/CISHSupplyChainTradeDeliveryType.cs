namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISHSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISHSupplyChainTradeDeliveryType
    {

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private QuantityType agreedQuantityField;

        private QuantityType dueInAvailableQuantityField;

        private QuantityType despatchedQuantityField;

        private QuantityType dueInForecastedQuantityField;

        private QuantityType modificationForecastedQuantityField;

        private QuantityType dueInRequestedQuantityField;

        private QuantityType remainingRequestedQuantityField;

        private QuantityType requestedQuantityField;

        private CITradePartyType inventoryManagerCITradePartyField;

        private CITradePartyType shipFromCITradePartyField;

        private CITradePartyType ultimateShipToCITradePartyField;

        private CITradePartyType shipToCITradePartyField;

        private CISupplyChainEventType ultimateShipToDeliveryCISupplyChainEventField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType deliveryNoteReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType despatchAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] receivingAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType shipmentScheduleReferencedCIReferencedDocumentField;

        private CIDeliveryInstructionsType[] deliveryCIDeliveryInstructionsField;

        private CISupplyChainEventType plannedShipFromDeliveryCISupplyChainEventField;

        private CISupplyChainEventType plannedShipToDeliveryCISupplyChainEventField;

        public IndicatorType PartialDeliveryAllowedIndicator
        {
            get => this.partialDeliveryAllowedIndicatorField;
            set => this.partialDeliveryAllowedIndicatorField = value;
        }

        public QuantityType AgreedQuantity
        {
            get => this.agreedQuantityField;
            set => this.agreedQuantityField = value;
        }

        public QuantityType DueInAvailableQuantity
        {
            get => this.dueInAvailableQuantityField;
            set => this.dueInAvailableQuantityField = value;
        }

        public QuantityType DespatchedQuantity
        {
            get => this.despatchedQuantityField;
            set => this.despatchedQuantityField = value;
        }

        public QuantityType DueInForecastedQuantity
        {
            get => this.dueInForecastedQuantityField;
            set => this.dueInForecastedQuantityField = value;
        }

        public QuantityType ModificationForecastedQuantity
        {
            get => this.modificationForecastedQuantityField;
            set => this.modificationForecastedQuantityField = value;
        }

        public QuantityType DueInRequestedQuantity
        {
            get => this.dueInRequestedQuantityField;
            set => this.dueInRequestedQuantityField = value;
        }

        public QuantityType RemainingRequestedQuantity
        {
            get => this.remainingRequestedQuantityField;
            set => this.remainingRequestedQuantityField = value;
        }

        public QuantityType RequestedQuantity
        {
            get => this.requestedQuantityField;
            set => this.requestedQuantityField = value;
        }

        public CITradePartyType InventoryManagerCITradeParty
        {
            get => this.inventoryManagerCITradePartyField;
            set => this.inventoryManagerCITradePartyField = value;
        }

        public CITradePartyType ShipFromCITradeParty
        {
            get => this.shipFromCITradePartyField;
            set => this.shipFromCITradePartyField = value;
        }

        public CITradePartyType UltimateShipToCITradeParty
        {
            get => this.ultimateShipToCITradePartyField;
            set => this.ultimateShipToCITradePartyField = value;
        }

        public CITradePartyType ShipToCITradeParty
        {
            get => this.shipToCITradePartyField;
            set => this.shipToCITradePartyField = value;
        }

        public CISupplyChainEventType UltimateShipToDeliveryCISupplyChainEvent
        {
            get => this.ultimateShipToDeliveryCISupplyChainEventField;
            set => this.ultimateShipToDeliveryCISupplyChainEventField = value;
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType DeliveryNoteReferencedCIReferencedDocument
        {
            get => this.deliveryNoteReferencedCIReferencedDocumentField;
            set => this.deliveryNoteReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType DespatchAdviceReferencedCIReferencedDocument
        {
            get => this.despatchAdviceReferencedCIReferencedDocumentField;
            set => this.despatchAdviceReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("ReceivingAdviceReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] ReceivingAdviceReferencedCIReferencedDocument
        {
            get => this.receivingAdviceReferencedCIReferencedDocumentField;
            set => this.receivingAdviceReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType ShipmentScheduleReferencedCIReferencedDocument
        {
            get => this.shipmentScheduleReferencedCIReferencedDocumentField;
            set => this.shipmentScheduleReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("DeliveryCIDeliveryInstructions")]
        public CIDeliveryInstructionsType[] DeliveryCIDeliveryInstructions
        {
            get => this.deliveryCIDeliveryInstructionsField;
            set => this.deliveryCIDeliveryInstructionsField = value;
        }

        public CISupplyChainEventType PlannedShipFromDeliveryCISupplyChainEvent
        {
            get => this.plannedShipFromDeliveryCISupplyChainEventField;
            set => this.plannedShipFromDeliveryCISupplyChainEventField = value;
        }

        public CISupplyChainEventType PlannedShipToDeliveryCISupplyChainEvent
        {
            get => this.plannedShipToDeliveryCISupplyChainEventField;
            set => this.plannedShipToDeliveryCISupplyChainEventField = value;
        }
    }
}