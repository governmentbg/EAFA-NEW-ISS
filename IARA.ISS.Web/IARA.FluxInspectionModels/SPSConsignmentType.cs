namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSConsignment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSConsignmentType
    {

        private IDType idField;

        private DateTimeType availabilityDueDateTimeField;

        private DateTimeType exportExitDateTimeField;

        private IndicatorType shipStoresIndicatorField;

        private SPSPartyType consignorSPSPartyField;

        private SPSPartyType consigneeSPSPartyField;

        private SPSCountryType exportSPSCountryField;

        private SPSCountryType[] reExportSPSCountryField;

        private SPSEventType[] storageSPSEventField;

        private SPSLocationType loadingBaseportSPSLocationField;

        private SPSCountryType importSPSCountryField;

        private SPSLocationType consigneeReceiptSPSLocationField;

        private SPSCountryType[] transitSPSCountryField;

        private SPSLocationType[] transitSPSLocationField;

        private SPSLocationType unloadingBaseportSPSLocationField;

        private SPSEventType examinationSPSEventField;

        private SPSPartyType carrierSPSPartyField;

        private SPSPartyType despatchSPSPartyField;

        private SPSPartyType deliverySPSPartyField;

        private SPSPartyType customsTransitAgentSPSPartyField;

        private SPSTransportMovementType[] mainCarriageSPSTransportMovementField;

        private SPSTransportEquipmentType[] utilizedSPSTransportEquipmentField;

        private SPSConsignmentItemType[] includedSPSConsignmentItemField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public DateTimeType AvailabilityDueDateTime
        {
            get => this.availabilityDueDateTimeField;
            set => this.availabilityDueDateTimeField = value;
        }

        public DateTimeType ExportExitDateTime
        {
            get => this.exportExitDateTimeField;
            set => this.exportExitDateTimeField = value;
        }

        public IndicatorType ShipStoresIndicator
        {
            get => this.shipStoresIndicatorField;
            set => this.shipStoresIndicatorField = value;
        }

        public SPSPartyType ConsignorSPSParty
        {
            get => this.consignorSPSPartyField;
            set => this.consignorSPSPartyField = value;
        }

        public SPSPartyType ConsigneeSPSParty
        {
            get => this.consigneeSPSPartyField;
            set => this.consigneeSPSPartyField = value;
        }

        public SPSCountryType ExportSPSCountry
        {
            get => this.exportSPSCountryField;
            set => this.exportSPSCountryField = value;
        }

        [XmlElement("ReExportSPSCountry")]
        public SPSCountryType[] ReExportSPSCountry
        {
            get => this.reExportSPSCountryField;
            set => this.reExportSPSCountryField = value;
        }

        [XmlElement("StorageSPSEvent")]
        public SPSEventType[] StorageSPSEvent
        {
            get => this.storageSPSEventField;
            set => this.storageSPSEventField = value;
        }

        public SPSLocationType LoadingBaseportSPSLocation
        {
            get => this.loadingBaseportSPSLocationField;
            set => this.loadingBaseportSPSLocationField = value;
        }

        public SPSCountryType ImportSPSCountry
        {
            get => this.importSPSCountryField;
            set => this.importSPSCountryField = value;
        }

        public SPSLocationType ConsigneeReceiptSPSLocation
        {
            get => this.consigneeReceiptSPSLocationField;
            set => this.consigneeReceiptSPSLocationField = value;
        }

        [XmlElement("TransitSPSCountry")]
        public SPSCountryType[] TransitSPSCountry
        {
            get => this.transitSPSCountryField;
            set => this.transitSPSCountryField = value;
        }

        [XmlElement("TransitSPSLocation")]
        public SPSLocationType[] TransitSPSLocation
        {
            get => this.transitSPSLocationField;
            set => this.transitSPSLocationField = value;
        }

        public SPSLocationType UnloadingBaseportSPSLocation
        {
            get => this.unloadingBaseportSPSLocationField;
            set => this.unloadingBaseportSPSLocationField = value;
        }

        public SPSEventType ExaminationSPSEvent
        {
            get => this.examinationSPSEventField;
            set => this.examinationSPSEventField = value;
        }

        public SPSPartyType CarrierSPSParty
        {
            get => this.carrierSPSPartyField;
            set => this.carrierSPSPartyField = value;
        }

        public SPSPartyType DespatchSPSParty
        {
            get => this.despatchSPSPartyField;
            set => this.despatchSPSPartyField = value;
        }

        public SPSPartyType DeliverySPSParty
        {
            get => this.deliverySPSPartyField;
            set => this.deliverySPSPartyField = value;
        }

        public SPSPartyType CustomsTransitAgentSPSParty
        {
            get => this.customsTransitAgentSPSPartyField;
            set => this.customsTransitAgentSPSPartyField = value;
        }

        [XmlElement("MainCarriageSPSTransportMovement")]
        public SPSTransportMovementType[] MainCarriageSPSTransportMovement
        {
            get => this.mainCarriageSPSTransportMovementField;
            set => this.mainCarriageSPSTransportMovementField = value;
        }

        [XmlElement("UtilizedSPSTransportEquipment")]
        public SPSTransportEquipmentType[] UtilizedSPSTransportEquipment
        {
            get => this.utilizedSPSTransportEquipmentField;
            set => this.utilizedSPSTransportEquipmentField = value;
        }

        [XmlElement("IncludedSPSConsignmentItem")]
        public SPSConsignmentItemType[] IncludedSPSConsignmentItem
        {
            get => this.includedSPSConsignmentItemField;
            set => this.includedSPSConsignmentItemField = value;
        }
    }
}