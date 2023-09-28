namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOLSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOLSupplyChainTradeDeliveryType
    {

        private QuantityType requestedQuantityField;

        private QuantityType agreedQuantityField;

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private QuantityType perPackageUnitQuantityField;

        private IDType idField;

        private IDType subordinateIDField;

        private QuantityType[] unavailableQuantityField;

        private QuantityType[] cancelledQuantityField;

        private QuantityType packageQuantityField;

        private QuantityType productUnitQuantityField;

        private CITradePartyType shipToCITradePartyField;

        private CITradePartyType shipFromCITradePartyField;

        private CIDeliveryInstructionsType deliveryCIDeliveryInstructionsField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CISupplyChainConsignmentType plannedCISupplyChainConsignmentField;

        private CISupplyChainEventType requestedDeliveryCISupplyChainEventField;

        private CISupplyChainEventType[] confirmedDeliveryCISupplyChainEventField;

        private CISupplyChainSupplyPlanType projectedCISupplyChainSupplyPlanField;

        public QuantityType RequestedQuantity
        {
            get => this.requestedQuantityField;
            set => this.requestedQuantityField = value;
        }

        public QuantityType AgreedQuantity
        {
            get => this.agreedQuantityField;
            set => this.agreedQuantityField = value;
        }

        public IndicatorType PartialDeliveryAllowedIndicator
        {
            get => this.partialDeliveryAllowedIndicatorField;
            set => this.partialDeliveryAllowedIndicatorField = value;
        }

        public QuantityType PerPackageUnitQuantity
        {
            get => this.perPackageUnitQuantityField;
            set => this.perPackageUnitQuantityField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType SubordinateID
        {
            get => this.subordinateIDField;
            set => this.subordinateIDField = value;
        }

        [XmlElement("UnavailableQuantity")]
        public QuantityType[] UnavailableQuantity
        {
            get => this.unavailableQuantityField;
            set => this.unavailableQuantityField = value;
        }

        [XmlElement("CancelledQuantity")]
        public QuantityType[] CancelledQuantity
        {
            get => this.cancelledQuantityField;
            set => this.cancelledQuantityField = value;
        }

        public QuantityType PackageQuantity
        {
            get => this.packageQuantityField;
            set => this.packageQuantityField = value;
        }

        public QuantityType ProductUnitQuantity
        {
            get => this.productUnitQuantityField;
            set => this.productUnitQuantityField = value;
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

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
        }

        public CISupplyChainConsignmentType PlannedCISupplyChainConsignment
        {
            get => this.plannedCISupplyChainConsignmentField;
            set => this.plannedCISupplyChainConsignmentField = value;
        }

        public CISupplyChainEventType RequestedDeliveryCISupplyChainEvent
        {
            get => this.requestedDeliveryCISupplyChainEventField;
            set => this.requestedDeliveryCISupplyChainEventField = value;
        }

        [XmlElement("ConfirmedDeliveryCISupplyChainEvent")]
        public CISupplyChainEventType[] ConfirmedDeliveryCISupplyChainEvent
        {
            get => this.confirmedDeliveryCISupplyChainEventField;
            set => this.confirmedDeliveryCISupplyChainEventField = value;
        }

        public CISupplyChainSupplyPlanType ProjectedCISupplyChainSupplyPlan
        {
            get => this.projectedCISupplyChainSupplyPlanField;
            set => this.projectedCISupplyChainSupplyPlanField = value;
        }
    }
}