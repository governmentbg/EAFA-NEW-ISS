namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISCRLSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISCRLSupplyChainTradeDeliveryType
    {

        private QuantityType agreedQuantityField;

        private QuantityType billedQuantityField;

        private QuantityType chargeFreeQuantityField;

        private MeasureType chargeableWeightMeasureField;

        private CodeType statusCodeField;

        private QuantityType despatchedQuantityField;

        private QuantityType dueInAvailableQuantityField;

        private QuantityType dueInForecastedQuantityField;

        private QuantityType dueInRequestedQuantityField;

        private QuantityType dueInReturnedQuantityField;

        private QuantityType economicOrderQuantityField;

        private IndicatorType finalDeliveryIndicatorField;

        private IndicatorType fullyDeliveredIndicatorField;

        private QuantityType gFMTransferRejectedQuantityField;

        private MeasureType grossVolumeMeasureField;

        private MeasureType grossWeightMeasureField;

        private QuantityType individualPackageQuantityField;

        private QuantityType modificationForecastedQuantityField;

        private MeasureType netVolumeMeasureField;

        private MeasureType netWeightMeasureField;

        private IndicatorType overDeliveryAllowedIndicatorField;

        private QuantityType packageQuantityField;

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private QuantityType perPackageUnitQuantityField;

        private QuantityType productUnitQuantityField;

        private QuantityType receivedQuantityField;

        private QuantityType remainingRequestedQuantityField;

        private QuantityType requestedQuantityField;

        private QuantityType reverseBilledQuantityField;

        private MeasureType theoreticalWeightMeasureField;

        private QuantityType turnInReceivedQuantityField;

        private FormattedDateTimeType formattedUltimateShipToDeliveryDateTimeField;

        private FormattedDateTimeType formattedPickUpAvailabilityDateTimeField;

        private CITradeCountryType finalDestinationCITradeCountryField;

        private CINoteType[] informationCINoteField;

        private CISupplyChainInventoryType[] availableCISupplyChainInventoryField;

        private CISupplyChainInventoryType[] consignmentCISupplyChainInventoryField;

        private CISupplyChainInventoryType[] customerCISupplyChainInventoryField;

        private CIReferencedDocumentType[] receivingAdviceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType deliveryNoteReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType despatchAdviceReferencedCIReferencedDocumentField;

        private CITradePartyType shipToCITradePartyField;

        private CITradePartyType shipFromCITradePartyField;

        private CITradePartyType ultimateShipToCITradePartyField;

        private CITradePartyType[] logisticsServiceProviderCITradePartyField;

        private CITradePartyType[] inventoryManagerCITradePartyField;

        private CISupplyChainPackagingType[] includedCISupplyChainPackagingField;

        private CISupplyChainScheduleType[] consumptionCISupplyChainScheduleField;

        private CISupplyChainScheduleType[] deliveryCISupplyChainScheduleField;

        private CISupplyChainScheduleType[] despatchCISupplyChainScheduleField;

        private CISupplyChainScheduleType[] orderCISupplyChainScheduleField;

        private CISupplyChainScheduleType[] receiptCISupplyChainScheduleField;

        private CISupplyChainScheduleType[] specifiedCISupplyChainScheduleField;

        private CISupplyChainScheduleType[] supplySpecifiedCISupplyChainScheduleField;

        public QuantityType AgreedQuantity
        {
            get => this.agreedQuantityField;
            set => this.agreedQuantityField = value;
        }

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

        public MeasureType ChargeableWeightMeasure
        {
            get => this.chargeableWeightMeasureField;
            set => this.chargeableWeightMeasureField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public QuantityType DespatchedQuantity
        {
            get => this.despatchedQuantityField;
            set => this.despatchedQuantityField = value;
        }

        public QuantityType DueInAvailableQuantity
        {
            get => this.dueInAvailableQuantityField;
            set => this.dueInAvailableQuantityField = value;
        }

        public QuantityType DueInForecastedQuantity
        {
            get => this.dueInForecastedQuantityField;
            set => this.dueInForecastedQuantityField = value;
        }

        public QuantityType DueInRequestedQuantity
        {
            get => this.dueInRequestedQuantityField;
            set => this.dueInRequestedQuantityField = value;
        }

        public QuantityType DueInReturnedQuantity
        {
            get => this.dueInReturnedQuantityField;
            set => this.dueInReturnedQuantityField = value;
        }

        public QuantityType EconomicOrderQuantity
        {
            get => this.economicOrderQuantityField;
            set => this.economicOrderQuantityField = value;
        }

        public IndicatorType FinalDeliveryIndicator
        {
            get => this.finalDeliveryIndicatorField;
            set => this.finalDeliveryIndicatorField = value;
        }

        public IndicatorType FullyDeliveredIndicator
        {
            get => this.fullyDeliveredIndicatorField;
            set => this.fullyDeliveredIndicatorField = value;
        }

        public QuantityType GFMTransferRejectedQuantity
        {
            get => this.gFMTransferRejectedQuantityField;
            set => this.gFMTransferRejectedQuantityField = value;
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

        public QuantityType IndividualPackageQuantity
        {
            get => this.individualPackageQuantityField;
            set => this.individualPackageQuantityField = value;
        }

        public QuantityType ModificationForecastedQuantity
        {
            get => this.modificationForecastedQuantityField;
            set => this.modificationForecastedQuantityField = value;
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

        public IndicatorType OverDeliveryAllowedIndicator
        {
            get => this.overDeliveryAllowedIndicatorField;
            set => this.overDeliveryAllowedIndicatorField = value;
        }

        public QuantityType PackageQuantity
        {
            get => this.packageQuantityField;
            set => this.packageQuantityField = value;
        }

        public IndicatorType PartialDeliveryAllowedIndicator
        {
            get => this.partialDeliveryAllowedIndicatorField;
            set => this.partialDeliveryAllowedIndicatorField = value;
        }

        public QuantityType PerPackageUnitQuantity
        {
            get => this.perPackageUnitQuantityField;
            set => this.perPackageUnitQuantityField = value;
        }

        public QuantityType ProductUnitQuantity
        {
            get => this.productUnitQuantityField;
            set => this.productUnitQuantityField = value;
        }

        public QuantityType ReceivedQuantity
        {
            get => this.receivedQuantityField;
            set => this.receivedQuantityField = value;
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

        public QuantityType ReverseBilledQuantity
        {
            get => this.reverseBilledQuantityField;
            set => this.reverseBilledQuantityField = value;
        }

        public MeasureType TheoreticalWeightMeasure
        {
            get => this.theoreticalWeightMeasureField;
            set => this.theoreticalWeightMeasureField = value;
        }

        public QuantityType TurnInReceivedQuantity
        {
            get => this.turnInReceivedQuantityField;
            set => this.turnInReceivedQuantityField = value;
        }

        public FormattedDateTimeType FormattedUltimateShipToDeliveryDateTime
        {
            get => this.formattedUltimateShipToDeliveryDateTimeField;
            set => this.formattedUltimateShipToDeliveryDateTimeField = value;
        }

        public FormattedDateTimeType FormattedPickUpAvailabilityDateTime
        {
            get => this.formattedPickUpAvailabilityDateTimeField;
            set => this.formattedPickUpAvailabilityDateTimeField = value;
        }

        public CITradeCountryType FinalDestinationCITradeCountry
        {
            get => this.finalDestinationCITradeCountryField;
            set => this.finalDestinationCITradeCountryField = value;
        }

        [XmlElement("InformationCINote")]
        public CINoteType[] InformationCINote
        {
            get => this.informationCINoteField;
            set => this.informationCINoteField = value;
        }

        [XmlElement("AvailableCISupplyChainInventory")]
        public CISupplyChainInventoryType[] AvailableCISupplyChainInventory
        {
            get => this.availableCISupplyChainInventoryField;
            set => this.availableCISupplyChainInventoryField = value;
        }

        [XmlElement("ConsignmentCISupplyChainInventory")]
        public CISupplyChainInventoryType[] ConsignmentCISupplyChainInventory
        {
            get => this.consignmentCISupplyChainInventoryField;
            set => this.consignmentCISupplyChainInventoryField = value;
        }

        [XmlElement("CustomerCISupplyChainInventory")]
        public CISupplyChainInventoryType[] CustomerCISupplyChainInventory
        {
            get => this.customerCISupplyChainInventoryField;
            set => this.customerCISupplyChainInventoryField = value;
        }

        [XmlElement("ReceivingAdviceReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] ReceivingAdviceReferencedCIReferencedDocument
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

        public CIReferencedDocumentType DespatchAdviceReferencedCIReferencedDocument
        {
            get => this.despatchAdviceReferencedCIReferencedDocumentField;
            set => this.despatchAdviceReferencedCIReferencedDocumentField = value;
        }

        public CITradePartyType ShipToCITradeParty
        {
            get => this.shipToCITradePartyField;
            set => this.shipToCITradePartyField = value;
        }

        public CITradePartyType ShipFromCITradeParty
        {
            get => this.shipFromCITradePartyField;
            set => this.shipFromCITradePartyField = value;
        }

        public CITradePartyType UltimateShipToCITradeParty
        {
            get => this.ultimateShipToCITradePartyField;
            set => this.ultimateShipToCITradePartyField = value;
        }

        [XmlElement("LogisticsServiceProviderCITradeParty")]
        public CITradePartyType[] LogisticsServiceProviderCITradeParty
        {
            get => this.logisticsServiceProviderCITradePartyField;
            set => this.logisticsServiceProviderCITradePartyField = value;
        }

        [XmlElement("InventoryManagerCITradeParty")]
        public CITradePartyType[] InventoryManagerCITradeParty
        {
            get => this.inventoryManagerCITradePartyField;
            set => this.inventoryManagerCITradePartyField = value;
        }

        [XmlElement("IncludedCISupplyChainPackaging")]
        public CISupplyChainPackagingType[] IncludedCISupplyChainPackaging
        {
            get => this.includedCISupplyChainPackagingField;
            set => this.includedCISupplyChainPackagingField = value;
        }

        [XmlElement("ConsumptionCISupplyChainSchedule")]
        public CISupplyChainScheduleType[] ConsumptionCISupplyChainSchedule
        {
            get => this.consumptionCISupplyChainScheduleField;
            set => this.consumptionCISupplyChainScheduleField = value;
        }

        [XmlElement("DeliveryCISupplyChainSchedule")]
        public CISupplyChainScheduleType[] DeliveryCISupplyChainSchedule
        {
            get => this.deliveryCISupplyChainScheduleField;
            set => this.deliveryCISupplyChainScheduleField = value;
        }

        [XmlElement("DespatchCISupplyChainSchedule")]
        public CISupplyChainScheduleType[] DespatchCISupplyChainSchedule
        {
            get => this.despatchCISupplyChainScheduleField;
            set => this.despatchCISupplyChainScheduleField = value;
        }

        [XmlElement("OrderCISupplyChainSchedule")]
        public CISupplyChainScheduleType[] OrderCISupplyChainSchedule
        {
            get => this.orderCISupplyChainScheduleField;
            set => this.orderCISupplyChainScheduleField = value;
        }

        [XmlElement("ReceiptCISupplyChainSchedule")]
        public CISupplyChainScheduleType[] ReceiptCISupplyChainSchedule
        {
            get => this.receiptCISupplyChainScheduleField;
            set => this.receiptCISupplyChainScheduleField = value;
        }

        [XmlElement("SpecifiedCISupplyChainSchedule")]
        public CISupplyChainScheduleType[] SpecifiedCISupplyChainSchedule
        {
            get => this.specifiedCISupplyChainScheduleField;
            set => this.specifiedCISupplyChainScheduleField = value;
        }

        [XmlElement("SupplySpecifiedCISupplyChainSchedule")]
        public CISupplyChainScheduleType[] SupplySpecifiedCISupplyChainSchedule
        {
            get => this.supplySpecifiedCISupplyChainScheduleField;
            set => this.supplySpecifiedCISupplyChainScheduleField = value;
        }
    }
}