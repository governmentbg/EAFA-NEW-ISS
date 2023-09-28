namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQHSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQHSupplyChainTradeDeliveryType
    {

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private CITradePartyType shipFromCITradePartyField;

        private CITradePartyType shipToCITradePartyField;

        private CIDeliveryInstructionsType[] deliveryCIDeliveryInstructionsField;

        private CIHandlingInstructionsType[] handlingCIHandlingInstructionsField;

        private CISupplyChainEventType[] plannedDeliveryCISupplyChainEventField;

        public IndicatorType PartialDeliveryAllowedIndicator
        {
            get => this.partialDeliveryAllowedIndicatorField;
            set => this.partialDeliveryAllowedIndicatorField = value;
        }

        public CITradePartyType ShipFromCITradeParty
        {
            get => this.shipFromCITradePartyField;
            set => this.shipFromCITradePartyField = value;
        }

        public CITradePartyType ShipToCITradeParty
        {
            get => this.shipToCITradePartyField;
            set => this.shipToCITradePartyField = value;
        }

        [XmlElement("DeliveryCIDeliveryInstructions")]
        public CIDeliveryInstructionsType[] DeliveryCIDeliveryInstructions
        {
            get => this.deliveryCIDeliveryInstructionsField;
            set => this.deliveryCIDeliveryInstructionsField = value;
        }

        [XmlElement("HandlingCIHandlingInstructions")]
        public CIHandlingInstructionsType[] HandlingCIHandlingInstructions
        {
            get => this.handlingCIHandlingInstructionsField;
            set => this.handlingCIHandlingInstructionsField = value;
        }

        [XmlElement("PlannedDeliveryCISupplyChainEvent")]
        public CISupplyChainEventType[] PlannedDeliveryCISupplyChainEvent
        {
            get => this.plannedDeliveryCISupplyChainEventField;
            set => this.plannedDeliveryCISupplyChainEventField = value;
        }
    }
}