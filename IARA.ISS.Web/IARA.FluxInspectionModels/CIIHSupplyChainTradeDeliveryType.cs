namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIIHSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIIHSupplyChainTradeDeliveryType
    {

        private CISupplyChainConsignmentType relatedCISupplyChainConsignmentField;

        private CITradePartyType shipToCITradePartyField;

        private CITradePartyType ultimateShipToCITradePartyField;

        private CITradePartyType shipFromCITradePartyField;

        private CISupplyChainEventType actualDespatchCISupplyChainEventField;

        private CISupplyChainEventType actualPickUpCISupplyChainEventField;

        private CISupplyChainEventType actualDeliveryCISupplyChainEventField;

        private CISupplyChainEventType actualReceiptCISupplyChainEventField;

        private CIReferencedDocumentType despatchAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType receivingAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType deliveryNoteReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType consumptionReportReferencedCIReferencedDocumentField;

        private CISupplyChainEventType[] previousDeliveryCISupplyChainEventField;

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

        [XmlElement("PreviousDeliveryCISupplyChainEvent")]
        public CISupplyChainEventType[] PreviousDeliveryCISupplyChainEvent
        {
            get => this.previousDeliveryCISupplyChainEventField;
            set => this.previousDeliveryCISupplyChainEventField = value;
        }
    }
}