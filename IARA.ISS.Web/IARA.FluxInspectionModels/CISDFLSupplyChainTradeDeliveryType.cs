namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISDFLSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISDFLSupplyChainTradeDeliveryType
    {

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private QuantityType agreedQuantityField;

        private QuantityType latestDespatchedQuantityField;

        private QuantityType remainingRequestedQuantityField;

        private QuantityType requestedQuantityField;

        private QuantityType perPackageUnitQuantityField;

        private CITradePartyType shipFromCITradePartyField;

        private CITradePartyType shipToCITradePartyField;

        private CITradeCountryType finalDestinationCITradeCountryField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType despatchAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] receivingAdviceReferencedCIReferencedDocumentField;

        private CISupplyChainSupplyPlanType[] projectedCISupplyChainSupplyPlanField;

        private CISupplyChainConsignmentType[] relatedCISupplyChainConsignmentField;

        private ReferencedLogisticsPackageType[] logisticsReferencedLogisticsPackageField;

        private CIHandlingInstructionsType[] handlingCIHandlingInstructionsField;

        private CISupplyChainEventType plannedShipToDeliveryCISupplyChainEventField;

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

        public QuantityType LatestDespatchedQuantity
        {
            get => this.latestDespatchedQuantityField;
            set => this.latestDespatchedQuantityField = value;
        }

        public QuantityType RemainingRequestedQuantity
        {
            get => this.remainingRequestedQuantityField;
            set => this.remainingRequestedQuantityField = value;
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

        public CITradeCountryType FinalDestinationCITradeCountry
        {
            get => this.finalDestinationCITradeCountryField;
            set => this.finalDestinationCITradeCountryField = value;
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

        [XmlElement("ProjectedCISupplyChainSupplyPlan")]
        public CISupplyChainSupplyPlanType[] ProjectedCISupplyChainSupplyPlan
        {
            get => this.projectedCISupplyChainSupplyPlanField;
            set => this.projectedCISupplyChainSupplyPlanField = value;
        }

        [XmlElement("RelatedCISupplyChainConsignment")]
        public CISupplyChainConsignmentType[] RelatedCISupplyChainConsignment
        {
            get => this.relatedCISupplyChainConsignmentField;
            set => this.relatedCISupplyChainConsignmentField = value;
        }

        [XmlElement("LogisticsReferencedLogisticsPackage")]
        public ReferencedLogisticsPackageType[] LogisticsReferencedLogisticsPackage
        {
            get => this.logisticsReferencedLogisticsPackageField;
            set => this.logisticsReferencedLogisticsPackageField = value;
        }

        [XmlElement("HandlingCIHandlingInstructions")]
        public CIHandlingInstructionsType[] HandlingCIHandlingInstructions
        {
            get => this.handlingCIHandlingInstructionsField;
            set => this.handlingCIHandlingInstructionsField = value;
        }

        public CISupplyChainEventType PlannedShipToDeliveryCISupplyChainEvent
        {
            get => this.plannedShipToDeliveryCISupplyChainEventField;
            set => this.plannedShipToDeliveryCISupplyChainEventField = value;
        }
    }
}