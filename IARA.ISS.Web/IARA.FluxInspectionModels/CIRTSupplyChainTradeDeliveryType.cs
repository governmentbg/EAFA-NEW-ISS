namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRTSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRTSupplyChainTradeDeliveryType
    {

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private CodeType quantityCalculationMethodCodeField;

        private CITradePartyType shipToCITradePartyField;

        private CISupplyChainEventType actualDespatchCISupplyChainEventField;

        private CISupplyChainEventType acceptanceCISupplyChainEventField;

        private CISupplyChainPackagingType[] includedCISupplyChainPackagingField;

        private CILogisticsTransportEquipmentType[] utilizedCILogisticsTransportEquipmentField;

        public IndicatorType PartialDeliveryAllowedIndicator
        {
            get => this.partialDeliveryAllowedIndicatorField;
            set => this.partialDeliveryAllowedIndicatorField = value;
        }

        public CodeType QuantityCalculationMethodCode
        {
            get => this.quantityCalculationMethodCodeField;
            set => this.quantityCalculationMethodCodeField = value;
        }

        public CITradePartyType ShipToCITradeParty
        {
            get => this.shipToCITradePartyField;
            set => this.shipToCITradePartyField = value;
        }

        public CISupplyChainEventType ActualDespatchCISupplyChainEvent
        {
            get => this.actualDespatchCISupplyChainEventField;
            set => this.actualDespatchCISupplyChainEventField = value;
        }

        public CISupplyChainEventType AcceptanceCISupplyChainEvent
        {
            get => this.acceptanceCISupplyChainEventField;
            set => this.acceptanceCISupplyChainEventField = value;
        }

        [XmlElement("IncludedCISupplyChainPackaging")]
        public CISupplyChainPackagingType[] IncludedCISupplyChainPackaging
        {
            get => this.includedCISupplyChainPackagingField;
            set => this.includedCISupplyChainPackagingField = value;
        }

        [XmlElement("UtilizedCILogisticsTransportEquipment")]
        public CILogisticsTransportEquipmentType[] UtilizedCILogisticsTransportEquipment
        {
            get => this.utilizedCILogisticsTransportEquipmentField;
            set => this.utilizedCILogisticsTransportEquipmentField = value;
        }
    }
}