namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILSupplyChainTradeDeliveryType
    {

        private QuantityType billedQuantityField;

        private QuantityType chargeFreeQuantityField;

        private QuantityType packageQuantityField;

        private QuantityType productUnitQuantityField;

        private QuantityType perPackageUnitQuantityField;

        private MeasureType netWeightMeasureField;

        private MeasureType grossWeightMeasureField;

        private MeasureType theoreticalWeightMeasureField;

        private QuantityType requestedQuantityField;

        private QuantityType receivedQuantityField;

        private CIDeliveryAdjustmentType[] specifiedCIDeliveryAdjustmentField;

        private CISupplyChainPackagingType[] includedCISupplyChainPackagingField;

        private CISupplyChainConsignmentType relatedCISupplyChainConsignmentField;

        private CITradePartyType shipToCITradePartyField;

        private CITradePartyType ultimateShipToCITradePartyField;

        private CITradePartyType shipFromCITradePartyField;

        private CISupplyChainEventType actualDespatchCISupplyChainEventField;

        private CISupplyChainEventType actualPickUpCISupplyChainEventField;

        private CISupplyChainEventType requestedDeliveryCISupplyChainEventField;

        private CISupplyChainEventType actualDeliveryCISupplyChainEventField;

        private CISupplyChainEventType actualReceiptCISupplyChainEventField;

        private CIReferencedDocumentType despatchAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType receivingAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType deliveryNoteReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType consumptionReportReferencedCIReferencedDocumentField;

        public QuantityType BilledQuantity
        {
            get => this.billedQuantityField;
            set => this.billedQuantityField = value;
        }

        public QuantityType ChargeFreeQuantity
        {
            get => this.chargeFreeQuantityField;
            set => this.chargeFreeQuantityField = value;
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

        public QuantityType PerPackageUnitQuantity
        {
            get => this.perPackageUnitQuantityField;
            set => this.perPackageUnitQuantityField = value;
        }

        public MeasureType NetWeightMeasure
        {
            get => this.netWeightMeasureField;
            set => this.netWeightMeasureField = value;
        }

        public MeasureType GrossWeightMeasure
        {
            get => this.grossWeightMeasureField;
            set => this.grossWeightMeasureField = value;
        }

        public MeasureType TheoreticalWeightMeasure
        {
            get => this.theoreticalWeightMeasureField;
            set => this.theoreticalWeightMeasureField = value;
        }

        public QuantityType RequestedQuantity
        {
            get => this.requestedQuantityField;
            set => this.requestedQuantityField = value;
        }

        public QuantityType ReceivedQuantity
        {
            get => this.receivedQuantityField;
            set => this.receivedQuantityField = value;
        }

        [XmlElement("SpecifiedCIDeliveryAdjustment")]
        public CIDeliveryAdjustmentType[] SpecifiedCIDeliveryAdjustment
        {
            get => this.specifiedCIDeliveryAdjustmentField;
            set => this.specifiedCIDeliveryAdjustmentField = value;
        }

        [XmlElement("IncludedCISupplyChainPackaging")]
        public CISupplyChainPackagingType[] IncludedCISupplyChainPackaging
        {
            get => this.includedCISupplyChainPackagingField;
            set => this.includedCISupplyChainPackagingField = value;
        }

        public CISupplyChainConsignmentType RelatedCISupplyChainConsignment
        {
            get => this.relatedCISupplyChainConsignmentField;
            set => this.relatedCISupplyChainConsignmentField = value;
        }

        public CITradePartyType ShipToCITradeParty
        {
            get => this.shipToCITradePartyField;
            set => this.shipToCITradePartyField = value;
        }

        public CITradePartyType UltimateShipToCITradeParty
        {
            get => this.ultimateShipToCITradePartyField;
            set => this.ultimateShipToCITradePartyField = value;
        }

        public CITradePartyType ShipFromCITradeParty
        {
            get => this.shipFromCITradePartyField;
            set => this.shipFromCITradePartyField = value;
        }

        public CISupplyChainEventType ActualDespatchCISupplyChainEvent
        {
            get => this.actualDespatchCISupplyChainEventField;
            set => this.actualDespatchCISupplyChainEventField = value;
        }

        public CISupplyChainEventType ActualPickUpCISupplyChainEvent
        {
            get => this.actualPickUpCISupplyChainEventField;
            set => this.actualPickUpCISupplyChainEventField = value;
        }

        public CISupplyChainEventType RequestedDeliveryCISupplyChainEvent
        {
            get => this.requestedDeliveryCISupplyChainEventField;
            set => this.requestedDeliveryCISupplyChainEventField = value;
        }

        public CISupplyChainEventType ActualDeliveryCISupplyChainEvent
        {
            get => this.actualDeliveryCISupplyChainEventField;
            set => this.actualDeliveryCISupplyChainEventField = value;
        }

        public CISupplyChainEventType ActualReceiptCISupplyChainEvent
        {
            get => this.actualReceiptCISupplyChainEventField;
            set => this.actualReceiptCISupplyChainEventField = value;
        }

        public CIReferencedDocumentType DespatchAdviceReferencedCIReferencedDocument
        {
            get => this.despatchAdviceReferencedCIReferencedDocumentField;
            set => this.despatchAdviceReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType ReceivingAdviceReferencedCIReferencedDocument
        {
            get => this.receivingAdviceReferencedCIReferencedDocumentField;
            set => this.receivingAdviceReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType DeliveryNoteReferencedCIReferencedDocument
        {
            get => this.deliveryNoteReferencedCIReferencedDocumentField;
            set => this.deliveryNoteReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType ConsumptionReportReferencedCIReferencedDocument
        {
            get => this.consumptionReportReferencedCIReferencedDocumentField;
            set => this.consumptionReportReferencedCIReferencedDocumentField = value;
        }
    }
}