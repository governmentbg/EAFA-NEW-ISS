namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISIFLSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISIFLSupplyChainTradeDeliveryType
    {

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private QuantityType agreedQuantityField;

        private QuantityType latestDespatchedQuantityField;

        private QuantityType remainingRequestedQuantityField;

        private QuantityType requestedQuantityField;

        private QuantityType[] availableQuantityField;

        private CITradePartyType shipFromCITradePartyField;

        private CITradePartyType shipToCITradePartyField;

        private CITradeCountryType finalDestinationCITradeCountryField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType despatchAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] receivingAdviceReferencedCIReferencedDocumentField;

        private CISupplyChainInventoryType[] availableCISupplyChainInventoryField;

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

        [XmlElement("AvailableQuantity")]
        public QuantityType[] AvailableQuantity
        {
            get => this.availableQuantityField;
            set => this.availableQuantityField = value;
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

        [XmlElement("AvailableCISupplyChainInventory")]
        public CISupplyChainInventoryType[] AvailableCISupplyChainInventory
        {
            get => this.availableCISupplyChainInventoryField;
            set => this.availableCISupplyChainInventoryField = value;
        }

        [XmlElement("ProjectedCISupplyChainSupplyPlan")]
        public CISupplyChainSupplyPlanType[] ProjectedCISupplyChainSupplyPlan
        {
            get => this.projectedCISupplyChainSupplyPlanField;
            set => this.projectedCISupplyChainSupplyPlanField = value;
        }
    }
}