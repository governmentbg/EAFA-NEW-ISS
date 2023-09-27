namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDHSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDHSupplyChainTradeDeliveryType
    {

        private IndicatorType finalDeliveryIndicatorField;

        private string pickUpAvailabilityDateTimeField;

        private IDType idField;

        private CITradePartyType shipToCITradePartyField;

        private CITradePartyType ultimateShipToCITradePartyField;

        private CITradePartyType shipFromCITradePartyField;

        private CIReferencedDocumentType shipmentScheduleReferencedCIReferencedDocumentField;

        private CISupplyChainConsignmentType[] relatedCISupplyChainConsignmentField;

        private CISupplyChainPackagingType[] includedCISupplyChainPackagingField;

        private CISupplyChainEventType actualDespatchCISupplyChainEventField;

        private CISupplyChainEventType confirmedReleaseCISupplyChainEventField;

        private CISupplyChainEventType requestedDeliveryCISupplyChainEventField;

        private CISupplyChainEventType plannedDeliveryCISupplyChainEventField;

        private CISupplyChainEventType actualLoadingCISupplyChainEventField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CINoteType[] informationCINoteField;

        public IndicatorType FinalDeliveryIndicator
        {
            get
            {
                return this.finalDeliveryIndicatorField;
            }
            set
            {
                this.finalDeliveryIndicatorField = value;
            }
        }

        public string PickUpAvailabilityDateTime
        {
            get
            {
                return this.pickUpAvailabilityDateTimeField;
            }
            set
            {
                this.pickUpAvailabilityDateTimeField = value;
            }
        }

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
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

        public CITradePartyType UltimateShipToCITradeParty
        {
            get
            {
                return this.ultimateShipToCITradePartyField;
            }
            set
            {
                this.ultimateShipToCITradePartyField = value;
            }
        }

        public CITradePartyType ShipFromCITradeParty
        {
            get
            {
                return this.shipFromCITradePartyField;
            }
            set
            {
                this.shipFromCITradePartyField = value;
            }
        }

        public CIReferencedDocumentType ShipmentScheduleReferencedCIReferencedDocument
        {
            get
            {
                return this.shipmentScheduleReferencedCIReferencedDocumentField;
            }
            set
            {
                this.shipmentScheduleReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("RelatedCISupplyChainConsignment")]
        public CISupplyChainConsignmentType[] RelatedCISupplyChainConsignment
        {
            get
            {
                return this.relatedCISupplyChainConsignmentField;
            }
            set
            {
                this.relatedCISupplyChainConsignmentField = value;
            }
        }

        [XmlElement("IncludedCISupplyChainPackaging")]
        public CISupplyChainPackagingType[] IncludedCISupplyChainPackaging
        {
            get
            {
                return this.includedCISupplyChainPackagingField;
            }
            set
            {
                this.includedCISupplyChainPackagingField = value;
            }
        }

        public CISupplyChainEventType ActualDespatchCISupplyChainEvent
        {
            get
            {
                return this.actualDespatchCISupplyChainEventField;
            }
            set
            {
                this.actualDespatchCISupplyChainEventField = value;
            }
        }

        public CISupplyChainEventType ConfirmedReleaseCISupplyChainEvent
        {
            get
            {
                return this.confirmedReleaseCISupplyChainEventField;
            }
            set
            {
                this.confirmedReleaseCISupplyChainEventField = value;
            }
        }

        public CISupplyChainEventType RequestedDeliveryCISupplyChainEvent
        {
            get
            {
                return this.requestedDeliveryCISupplyChainEventField;
            }
            set
            {
                this.requestedDeliveryCISupplyChainEventField = value;
            }
        }

        public CISupplyChainEventType PlannedDeliveryCISupplyChainEvent
        {
            get
            {
                return this.plannedDeliveryCISupplyChainEventField;
            }
            set
            {
                this.plannedDeliveryCISupplyChainEventField = value;
            }
        }

        public CISupplyChainEventType ActualLoadingCISupplyChainEvent
        {
            get
            {
                return this.actualLoadingCISupplyChainEventField;
            }
            set
            {
                this.actualLoadingCISupplyChainEventField = value;
            }
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get
            {
                return this.additionalReferencedCIReferencedDocumentField;
            }
            set
            {
                this.additionalReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("InformationCINote")]
        public CINoteType[] InformationCINote
        {
            get
            {
                return this.informationCINoteField;
            }
            set
            {
                this.informationCINoteField = value;
            }
        }
    }
}