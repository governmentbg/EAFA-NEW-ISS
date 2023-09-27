namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISSNLSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISSNLSupplyChainTradeDeliveryType
    {

        private IndicatorType finalDeliveryIndicatorField;

        private MeasureType grossVolumeMeasureField;

        private MeasureType grossWeightMeasureField;

        private MeasureType netVolumeMeasureField;

        private MeasureType netWeightMeasureField;

        private QuantityType agreedQuantityField;

        private QuantityType latestDespatchedQuantityField;

        private CITradePartyType shipFromCITradePartyField;

        private CITradePartyType shipToCITradePartyField;

        private CISupplyChainEventType[] actualDeliveryCISupplyChainEventField;

        private CISupplyChainEventType[] plannedDeliveryCISupplyChainEventField;

        private CISupplyChainEventType[] actualDespatchCISupplyChainEventField;

        private CISupplyChainEventType[] plannedDespatchCISupplyChainEventField;

        private CIDeliveryAdjustmentType[] specifiedCIDeliveryAdjustmentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType despatchAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] receivingAdviceReferencedCIReferencedDocumentField;

        public IndicatorType FinalDeliveryIndicator
        {
            get => this.finalDeliveryIndicatorField;
            set => this.finalDeliveryIndicatorField = value;
        }

        public MeasureType GrossVolumeMeasure
        {
            get => this.grossVolumeMeasureField;
            set => this.grossVolumeMeasureField = value;
        }

        public MeasureType GrossWeightMeasure
        {
            get => this.grossWeightMeasureField;
            set => this.grossWeightMeasureField = value;
        }

        public MeasureType NetVolumeMeasure
        {
            get => this.netVolumeMeasureField;
            set => this.netVolumeMeasureField = value;
        }

        public MeasureType NetWeightMeasure
        {
            get => this.netWeightMeasureField;
            set => this.netWeightMeasureField = value;
        }

        public QuantityType AgreedQuantity
        {
            get => this.agreedQuantityField;
            set => this.agreedQuantityField = value;
        }

        public QuantityType LatestDespatchedQuantity
        {
            get => this.latestDespatchedQuantityField;
            set => this.latestDespatchedQuantityField = value;
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

        [XmlElement("ActualDeliveryCISupplyChainEvent")]
        public CISupplyChainEventType[] ActualDeliveryCISupplyChainEvent
        {
            get => this.actualDeliveryCISupplyChainEventField;
            set => this.actualDeliveryCISupplyChainEventField = value;
        }

        [XmlElement("PlannedDeliveryCISupplyChainEvent")]
        public CISupplyChainEventType[] PlannedDeliveryCISupplyChainEvent
        {
            get => this.plannedDeliveryCISupplyChainEventField;
            set => this.plannedDeliveryCISupplyChainEventField = value;
        }

        [XmlElement("ActualDespatchCISupplyChainEvent")]
        public CISupplyChainEventType[] ActualDespatchCISupplyChainEvent
        {
            get => this.actualDespatchCISupplyChainEventField;
            set => this.actualDespatchCISupplyChainEventField = value;
        }

        [XmlElement("PlannedDespatchCISupplyChainEvent")]
        public CISupplyChainEventType[] PlannedDespatchCISupplyChainEvent
        {
            get => this.plannedDespatchCISupplyChainEventField;
            set => this.plannedDespatchCISupplyChainEventField = value;
        }

        [XmlElement("SpecifiedCIDeliveryAdjustment")]
        public CIDeliveryAdjustmentType[] SpecifiedCIDeliveryAdjustment
        {
            get => this.specifiedCIDeliveryAdjustmentField;
            set => this.specifiedCIDeliveryAdjustmentField = value;
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
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
    }
}