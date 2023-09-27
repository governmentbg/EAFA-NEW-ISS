namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQLSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQLSupplyChainTradeDeliveryType
    {

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private QuantityType agreedQuantityField;

        private QuantityType requestedQuantityField;

        private QuantityType perPackageUnitQuantityField;

        private QuantityType packageQuantityField;

        private QuantityType productUnitQuantityField;

        private CITradePartyType shipFromCITradePartyField;

        private CITradePartyType shipToCITradePartyField;

        private CISupplyChainEventType plannedDeliveryCISupplyChainEventField;

        private CISupplyChainEventType requestedDeliveryCISupplyChainEventField;

        private CISupplyChainEventType plannedDespatchCISupplyChainEventField;

        private CISupplyChainEventType requestedDespatchCISupplyChainEventField;

        private CISupplyChainSupplyPlanType[] projectedCISupplyChainSupplyPlanField;

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

        public QuantityType RequestedQuantity
        {
            get => this.requestedQuantityField;
            set => this.requestedQuantityField = value;
        }

        public QuantityType PerPackageUnitQuantity
        {
            get => this.perPackageUnitQuantityField;
            set => this.perPackageUnitQuantityField = value;
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

        public CISupplyChainEventType PlannedDeliveryCISupplyChainEvent
        {
            get => this.plannedDeliveryCISupplyChainEventField;
            set => this.plannedDeliveryCISupplyChainEventField = value;
        }

        public CISupplyChainEventType RequestedDeliveryCISupplyChainEvent
        {
            get => this.requestedDeliveryCISupplyChainEventField;
            set => this.requestedDeliveryCISupplyChainEventField = value;
        }

        public CISupplyChainEventType PlannedDespatchCISupplyChainEvent
        {
            get => this.plannedDespatchCISupplyChainEventField;
            set => this.plannedDespatchCISupplyChainEventField = value;
        }

        public CISupplyChainEventType RequestedDespatchCISupplyChainEvent
        {
            get => this.requestedDespatchCISupplyChainEventField;
            set => this.requestedDespatchCISupplyChainEventField = value;
        }

        [XmlElement("ProjectedCISupplyChainSupplyPlan")]
        public CISupplyChainSupplyPlanType[] ProjectedCISupplyChainSupplyPlan
        {
            get => this.projectedCISupplyChainSupplyPlanField;
            set => this.projectedCISupplyChainSupplyPlanField = value;
        }
    }
}