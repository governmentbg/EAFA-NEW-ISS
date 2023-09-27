namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOHSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOHSupplyChainTradeDeliveryType
    {

        private IDType subordinateIDField;

        private CITradePartyType shipToCITradePartyField;

        private CITradePartyType shipFromCITradePartyField;

        private CIDeliveryInstructionsType deliveryCIDeliveryInstructionsField;

        private CISupplyChainEventType plannedDespatchCISupplyChainEventField;

        private CISupplyChainEventType plannedReleaseCISupplyChainEventField;

        private CISupplyChainEventType plannedDeliveryCISupplyChainEventField;

        private CISupplyChainConsignmentType[] plannedCISupplyChainConsignmentField;

        public IDType SubordinateID
        {
            get => this.subordinateIDField;
            set => this.subordinateIDField = value;
        }

        public CITradePartyType ShipToCITradeParty
        {
            get => this.shipToCITradePartyField;
            set => this.shipToCITradePartyField = value;
        }

        public CITradePartyType ShipFromCITradeParty
        {
            get => this.shipFromCITradePartyField;
            set => this.shipFromCITradePartyField = value;
        }

        public CIDeliveryInstructionsType DeliveryCIDeliveryInstructions
        {
            get => this.deliveryCIDeliveryInstructionsField;
            set => this.deliveryCIDeliveryInstructionsField = value;
        }

        public CISupplyChainEventType PlannedDespatchCISupplyChainEvent
        {
            get => this.plannedDespatchCISupplyChainEventField;
            set => this.plannedDespatchCISupplyChainEventField = value;
        }

        public CISupplyChainEventType PlannedReleaseCISupplyChainEvent
        {
            get => this.plannedReleaseCISupplyChainEventField;
            set => this.plannedReleaseCISupplyChainEventField = value;
        }

        public CISupplyChainEventType PlannedDeliveryCISupplyChainEvent
        {
            get => this.plannedDeliveryCISupplyChainEventField;
            set => this.plannedDeliveryCISupplyChainEventField = value;
        }

        [XmlElement("PlannedCISupplyChainConsignment")]
        public CISupplyChainConsignmentType[] PlannedCISupplyChainConsignment
        {
            get => this.plannedCISupplyChainConsignmentField;
            set => this.plannedCISupplyChainConsignmentField = value;
        }
    }
}