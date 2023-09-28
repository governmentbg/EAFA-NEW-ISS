namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILBSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILBSupplyChainTradeDeliveryType
    {

        private QuantityType packageQuantityField;

        private QuantityType productUnitQuantityField;

        private QuantityType perPackageUnitQuantityField;

        private QuantityType billedQuantityField;

        private CISupplyChainPackagingType[] includedCISupplyChainPackagingField;

        private CISupplyChainEventType actualDeliveryCISupplyChainEventField;

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

        public QuantityType PerPackageUnitQuantity
        {
            get => this.perPackageUnitQuantityField;
            set => this.perPackageUnitQuantityField = value;
        }

        public QuantityType BilledQuantity
        {
            get => this.billedQuantityField;
            set => this.billedQuantityField = value;
        }

        [XmlElement("IncludedCISupplyChainPackaging")]
        public CISupplyChainPackagingType[] IncludedCISupplyChainPackaging
        {
            get => this.includedCISupplyChainPackagingField;
            set => this.includedCISupplyChainPackagingField = value;
        }

        public CISupplyChainEventType ActualDeliveryCISupplyChainEvent
        {
            get => this.actualDeliveryCISupplyChainEventField;
            set => this.actualDeliveryCISupplyChainEventField = value;
        }
    }
}