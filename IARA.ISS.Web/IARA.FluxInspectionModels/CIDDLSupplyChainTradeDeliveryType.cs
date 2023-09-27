namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDLSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDLSupplyChainTradeDeliveryType
    {

        private QuantityType billedQuantityField;

        private QuantityType chargeFreeQuantityField;

        private QuantityType packageQuantityField;

        private QuantityType productUnitQuantityField;

        private QuantityType perPackageUnitQuantityField;

        private MeasureType netWeightMeasureField;

        private MeasureType grossWeightMeasureField;

        private MeasureType theoreticalWeightMeasureField;

        private string ultimateShipToDeliveryDateTimeField;

        private QuantityType despatchedQuantityField;

        private QuantityType requestedQuantityField;

        private QuantityType remainingRequestedQuantityField;

        private CITradePartyType shipFromCITradePartyField;

        private CITradePartyType shipToCITradePartyField;

        private CITradePartyType ultimateShipToCITradePartyField;

        private CISupplyChainConsignmentType[] relatedCISupplyChainConsignmentField;

        private CIHandlingInstructionsType[] handlingCIHandlingInstructionsField;

        private CISupplyChainEventType actualDespatchCISupplyChainEventField;

        private CISupplyChainEventType actualPickUpCISupplyChainEventField;

        private CISupplyChainEventType requestedDeliveryCISupplyChainEventField;

        private CIDeliveryAdjustmentType specifiedCIDeliveryAdjustmentField;

        private CISupplyChainPackagingType[] includedCISupplyChainPackagingField;

        private CISupplyChainSupplyPlanType[] projectedCISupplyChainSupplyPlanField;

        public QuantityType BilledQuantity
        {
            get
            {
                return this.billedQuantityField;
            }
            set
            {
                this.billedQuantityField = value;
            }
        }

        public QuantityType ChargeFreeQuantity
        {
            get
            {
                return this.chargeFreeQuantityField;
            }
            set
            {
                this.chargeFreeQuantityField = value;
            }
        }

        public QuantityType PackageQuantity
        {
            get
            {
                return this.packageQuantityField;
            }
            set
            {
                this.packageQuantityField = value;
            }
        }

        public QuantityType ProductUnitQuantity
        {
            get
            {
                return this.productUnitQuantityField;
            }
            set
            {
                this.productUnitQuantityField = value;
            }
        }

        public QuantityType PerPackageUnitQuantity
        {
            get
            {
                return this.perPackageUnitQuantityField;
            }
            set
            {
                this.perPackageUnitQuantityField = value;
            }
        }

        public MeasureType NetWeightMeasure
        {
            get
            {
                return this.netWeightMeasureField;
            }
            set
            {
                this.netWeightMeasureField = value;
            }
        }

        public MeasureType GrossWeightMeasure
        {
            get
            {
                return this.grossWeightMeasureField;
            }
            set
            {
                this.grossWeightMeasureField = value;
            }
        }

        public MeasureType TheoreticalWeightMeasure
        {
            get
            {
                return this.theoreticalWeightMeasureField;
            }
            set
            {
                this.theoreticalWeightMeasureField = value;
            }
        }

        public string UltimateShipToDeliveryDateTime
        {
            get
            {
                return this.ultimateShipToDeliveryDateTimeField;
            }
            set
            {
                this.ultimateShipToDeliveryDateTimeField = value;
            }
        }

        public QuantityType DespatchedQuantity
        {
            get
            {
                return this.despatchedQuantityField;
            }
            set
            {
                this.despatchedQuantityField = value;
            }
        }

        public QuantityType RequestedQuantity
        {
            get
            {
                return this.requestedQuantityField;
            }
            set
            {
                this.requestedQuantityField = value;
            }
        }

        public QuantityType RemainingRequestedQuantity
        {
            get
            {
                return this.remainingRequestedQuantityField;
            }
            set
            {
                this.remainingRequestedQuantityField = value;
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

        [XmlElement("HandlingCIHandlingInstructions")]
        public CIHandlingInstructionsType[] HandlingCIHandlingInstructions
        {
            get
            {
                return this.handlingCIHandlingInstructionsField;
            }
            set
            {
                this.handlingCIHandlingInstructionsField = value;
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

        public CISupplyChainEventType ActualPickUpCISupplyChainEvent
        {
            get
            {
                return this.actualPickUpCISupplyChainEventField;
            }
            set
            {
                this.actualPickUpCISupplyChainEventField = value;
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

        public CIDeliveryAdjustmentType SpecifiedCIDeliveryAdjustment
        {
            get
            {
                return this.specifiedCIDeliveryAdjustmentField;
            }
            set
            {
                this.specifiedCIDeliveryAdjustmentField = value;
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

        [XmlElement("ProjectedCISupplyChainSupplyPlan")]
        public CISupplyChainSupplyPlanType[] ProjectedCISupplyChainSupplyPlan
        {
            get
            {
                return this.projectedCISupplyChainSupplyPlanField;
            }
            set
            {
                this.projectedCISupplyChainSupplyPlanField = value;
            }
        }
    }
}