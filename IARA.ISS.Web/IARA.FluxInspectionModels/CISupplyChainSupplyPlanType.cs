namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISupplyChainSupplyPlan", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISupplyChainSupplyPlanType
    {

        private CommitmentLevelCodeType commitmentLevelCodeField;

        private QuantityType plannedQuantityField;

        private QuantityType actualQuantityField;

        private QuantityType toleranceQuantityField;

        private QuantityType minusToleranceQuantityField;

        private QuantityType plusToleranceQuantityField;

        private DateTimeType synchronizationDateTimeField;

        private QuantityType synchronizationQuantityField;

        private CISpecifiedPeriodType specifiedCISpecifiedPeriodField;

        private CISupplyChainEventType[] deliveryCISupplyChainEventField;

        private CILogisticsLocationType[] specifiedCILogisticsLocationField;

        private CIReferencedDocumentType[] contractReferencedCIReferencedDocumentField;

        private CISupplyChainEventType[] scheduledDeliveryCISupplyChainEventField;

        private CISupplyChainEventType[] confirmedDeliveryCISupplyChainEventField;

        private CIReferencedDocumentType[] deliveryNoteReferencedCIReferencedDocumentField;

        private CITradePartyType shipToCITradePartyField;

        public CommitmentLevelCodeType CommitmentLevelCode
        {
            get
            {
                return this.commitmentLevelCodeField;
            }
            set
            {
                this.commitmentLevelCodeField = value;
            }
        }

        public QuantityType PlannedQuantity
        {
            get
            {
                return this.plannedQuantityField;
            }
            set
            {
                this.plannedQuantityField = value;
            }
        }

        public QuantityType ActualQuantity
        {
            get
            {
                return this.actualQuantityField;
            }
            set
            {
                this.actualQuantityField = value;
            }
        }

        public QuantityType ToleranceQuantity
        {
            get
            {
                return this.toleranceQuantityField;
            }
            set
            {
                this.toleranceQuantityField = value;
            }
        }

        public QuantityType MinusToleranceQuantity
        {
            get
            {
                return this.minusToleranceQuantityField;
            }
            set
            {
                this.minusToleranceQuantityField = value;
            }
        }

        public QuantityType PlusToleranceQuantity
        {
            get
            {
                return this.plusToleranceQuantityField;
            }
            set
            {
                this.plusToleranceQuantityField = value;
            }
        }

        public DateTimeType SynchronizationDateTime
        {
            get
            {
                return this.synchronizationDateTimeField;
            }
            set
            {
                this.synchronizationDateTimeField = value;
            }
        }

        public QuantityType SynchronizationQuantity
        {
            get
            {
                return this.synchronizationQuantityField;
            }
            set
            {
                this.synchronizationQuantityField = value;
            }
        }

        public CISpecifiedPeriodType SpecifiedCISpecifiedPeriod
        {
            get
            {
                return this.specifiedCISpecifiedPeriodField;
            }
            set
            {
                this.specifiedCISpecifiedPeriodField = value;
            }
        }

        [XmlElement("DeliveryCISupplyChainEvent")]
        public CISupplyChainEventType[] DeliveryCISupplyChainEvent
        {
            get
            {
                return this.deliveryCISupplyChainEventField;
            }
            set
            {
                this.deliveryCISupplyChainEventField = value;
            }
        }

        [XmlElement("SpecifiedCILogisticsLocation")]
        public CILogisticsLocationType[] SpecifiedCILogisticsLocation
        {
            get
            {
                return this.specifiedCILogisticsLocationField;
            }
            set
            {
                this.specifiedCILogisticsLocationField = value;
            }
        }

        [XmlElement("ContractReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] ContractReferencedCIReferencedDocument
        {
            get
            {
                return this.contractReferencedCIReferencedDocumentField;
            }
            set
            {
                this.contractReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("ScheduledDeliveryCISupplyChainEvent")]
        public CISupplyChainEventType[] ScheduledDeliveryCISupplyChainEvent
        {
            get
            {
                return this.scheduledDeliveryCISupplyChainEventField;
            }
            set
            {
                this.scheduledDeliveryCISupplyChainEventField = value;
            }
        }

        [XmlElement("ConfirmedDeliveryCISupplyChainEvent")]
        public CISupplyChainEventType[] ConfirmedDeliveryCISupplyChainEvent
        {
            get
            {
                return this.confirmedDeliveryCISupplyChainEventField;
            }
            set
            {
                this.confirmedDeliveryCISupplyChainEventField = value;
            }
        }

        [XmlElement("DeliveryNoteReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] DeliveryNoteReferencedCIReferencedDocument
        {
            get
            {
                return this.deliveryNoteReferencedCIReferencedDocumentField;
            }
            set
            {
                this.deliveryNoteReferencedCIReferencedDocumentField = value;
            }
        }

        public CITradePartyType ShipToCITradeParty
        {
            get
            {
                return this.shipToCITradePartyField;
            }
            set
            {
                this.shipToCITradePartyField = value;
            }
        }
    }
}