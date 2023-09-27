namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRLSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRLSupplyChainTradeDeliveryType
    {

        private QuantityType requestedQuantityField;

        private QuantityType agreedQuantityField;

        private QuantityType billedQuantityField;

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private CodeType quantityCalculationMethodCodeField;

        private QuantityType perPackageUnitQuantityField;

        private CISupplyChainEventType requestedDeliveryCISupplyChainEventField;

        private CISupplyChainEventType actualDespatchCISupplyChainEventField;

        private CISupplyChainEventType acceptanceCISupplyChainEventField;

        private CITradePartyType shipToCITradePartyField;

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

        public QuantityType BilledQuantity
        {
            get => this.billedQuantityField;
            set => this.billedQuantityField = value;
        }

        public IndicatorType PartialDeliveryAllowedIndicator
        {
            get => this.partialDeliveryAllowedIndicatorField;
            set => this.partialDeliveryAllowedIndicatorField = value;
        }

        public CodeType QuantityCalculationMethodCode
        {
            get => this.quantityCalculationMethodCodeField;
            set => this.quantityCalculationMethodCodeField = value;
        }

        public QuantityType PerPackageUnitQuantity
        {
            get => this.perPackageUnitQuantityField;
            set => this.perPackageUnitQuantityField = value;
        }

        public CISupplyChainEventType RequestedDeliveryCISupplyChainEvent
        {
            get => this.requestedDeliveryCISupplyChainEventField;
            set => this.requestedDeliveryCISupplyChainEventField = value;
        }

        public CISupplyChainEventType ActualDespatchCISupplyChainEvent
        {
            get => this.actualDespatchCISupplyChainEventField;
            set => this.actualDespatchCISupplyChainEventField = value;
        }

        public CISupplyChainEventType AcceptanceCISupplyChainEvent
        {
            get => this.acceptanceCISupplyChainEventField;
            set => this.acceptanceCISupplyChainEventField = value;
        }

        public CITradePartyType ShipToCITradeParty
        {
            get => this.shipToCITradePartyField;
            set => this.shipToCITradePartyField = value;
        }
    }
}