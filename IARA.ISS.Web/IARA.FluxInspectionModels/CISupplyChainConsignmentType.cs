namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISupplyChainConsignment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISupplyChainConsignmentType
    {

        private AmountType[] associatedInvoiceAmountField;

        private AmountType declaredValueForCustomsAmountField;

        private AmountType totalChargeAmountField;

        private QuantityType packageQuantityField;

        private WeightUnitMeasureType[] grossWeightMeasureField;

        private WeightUnitMeasureType[] netWeightMeasureField;

        private VolumeUnitMeasureType[] grossVolumeMeasureField;

        private VolumeUnitMeasureType netVolumeMeasureField;

        private WeightUnitMeasureType[] chargeableWeightMeasureField;

        private CITradePartyType consignorCITradePartyField;

        private CITradePartyType consigneeCITradePartyField;

        private CITradePartyType deliveryCITradePartyField;

        private CILogisticsTransportMovementType[] specifiedCILogisticsTransportMovementField;

        private CITradePartyType customsImportAgentCITradePartyField;

        private CITradePartyType customsExportAgentCITradePartyField;

        private CITradePartyType carrierCITradePartyField;

        private CIReferencedDocumentType transportContractCIReferencedDocumentField;

        private CILogisticsTransportEquipmentType[] utilizedCILogisticsTransportEquipmentField;

        private CITradePartyType freightForwarderCITradePartyField;

        private CITradePartyType[] groupingCentreCITradePartyField;

        private CISupplyChainConsignmentItemType[] includedCISupplyChainConsignmentItemField;

        private CIReferencedDocumentType[] associatedCIReferencedDocumentField;

        private CITransportCargoInsuranceType applicableCITransportCargoInsuranceField;

        private CICrossBorderCustomsValuationType applicableCICrossBorderCustomsValuationField;

        private CICrossBorderRegulatoryProcedureType[] applicableCICrossBorderRegulatoryProcedureField;

        private CILogisticsLocationType transitCILogisticsLocationField;

        [XmlElement("AssociatedInvoiceAmount")]
        public AmountType[] AssociatedInvoiceAmount
        {
            get
            {
                return this.associatedInvoiceAmountField;
            }
            set
            {
                this.associatedInvoiceAmountField = value;
            }
        }

        public AmountType DeclaredValueForCustomsAmount
        {
            get
            {
                return this.declaredValueForCustomsAmountField;
            }
            set
            {
                this.declaredValueForCustomsAmountField = value;
            }
        }

        public AmountType TotalChargeAmount
        {
            get
            {
                return this.totalChargeAmountField;
            }
            set
            {
                this.totalChargeAmountField = value;
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

        [XmlElement("GrossWeightMeasure")]
        public WeightUnitMeasureType[] GrossWeightMeasure
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

        [XmlElement("NetWeightMeasure")]
        public WeightUnitMeasureType[] NetWeightMeasure
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

        [XmlElement("GrossVolumeMeasure")]
        public VolumeUnitMeasureType[] GrossVolumeMeasure
        {
            get
            {
                return this.grossVolumeMeasureField;
            }
            set
            {
                this.grossVolumeMeasureField = value;
            }
        }

        public VolumeUnitMeasureType NetVolumeMeasure
        {
            get
            {
                return this.netVolumeMeasureField;
            }
            set
            {
                this.netVolumeMeasureField = value;
            }
        }

        [XmlElement("ChargeableWeightMeasure")]
        public WeightUnitMeasureType[] ChargeableWeightMeasure
        {
            get
            {
                return this.chargeableWeightMeasureField;
            }
            set
            {
                this.chargeableWeightMeasureField = value;
            }
        }

        public CITradePartyType ConsignorCITradeParty
        {
            get
            {
                return this.consignorCITradePartyField;
            }
            set
            {
                this.consignorCITradePartyField = value;
            }
        }

        public CITradePartyType ConsigneeCITradeParty
        {
            get
            {
                return this.consigneeCITradePartyField;
            }
            set
            {
                this.consigneeCITradePartyField = value;
            }
        }

        public CITradePartyType DeliveryCITradeParty
        {
            get
            {
                return this.deliveryCITradePartyField;
            }
            set
            {
                this.deliveryCITradePartyField = value;
            }
        }

        [XmlElement("SpecifiedCILogisticsTransportMovement")]
        public CILogisticsTransportMovementType[] SpecifiedCILogisticsTransportMovement
        {
            get
            {
                return this.specifiedCILogisticsTransportMovementField;
            }
            set
            {
                this.specifiedCILogisticsTransportMovementField = value;
            }
        }

        public CITradePartyType CustomsImportAgentCITradeParty
        {
            get
            {
                return this.customsImportAgentCITradePartyField;
            }
            set
            {
                this.customsImportAgentCITradePartyField = value;
            }
        }

        public CITradePartyType CustomsExportAgentCITradeParty
        {
            get
            {
                return this.customsExportAgentCITradePartyField;
            }
            set
            {
                this.customsExportAgentCITradePartyField = value;
            }
        }

        public CITradePartyType CarrierCITradeParty
        {
            get
            {
                return this.carrierCITradePartyField;
            }
            set
            {
                this.carrierCITradePartyField = value;
            }
        }

        public CIReferencedDocumentType TransportContractCIReferencedDocument
        {
            get
            {
                return this.transportContractCIReferencedDocumentField;
            }
            set
            {
                this.transportContractCIReferencedDocumentField = value;
            }
        }

        [XmlElement("UtilizedCILogisticsTransportEquipment")]
        public CILogisticsTransportEquipmentType[] UtilizedCILogisticsTransportEquipment
        {
            get
            {
                return this.utilizedCILogisticsTransportEquipmentField;
            }
            set
            {
                this.utilizedCILogisticsTransportEquipmentField = value;
            }
        }

        public CITradePartyType FreightForwarderCITradeParty
        {
            get
            {
                return this.freightForwarderCITradePartyField;
            }
            set
            {
                this.freightForwarderCITradePartyField = value;
            }
        }

        [XmlElement("GroupingCentreCITradeParty")]
        public CITradePartyType[] GroupingCentreCITradeParty
        {
            get
            {
                return this.groupingCentreCITradePartyField;
            }
            set
            {
                this.groupingCentreCITradePartyField = value;
            }
        }

        [XmlElement("IncludedCISupplyChainConsignmentItem")]
        public CISupplyChainConsignmentItemType[] IncludedCISupplyChainConsignmentItem
        {
            get
            {
                return this.includedCISupplyChainConsignmentItemField;
            }
            set
            {
                this.includedCISupplyChainConsignmentItemField = value;
            }
        }

        [XmlElement("AssociatedCIReferencedDocument")]
        public CIReferencedDocumentType[] AssociatedCIReferencedDocument
        {
            get
            {
                return this.associatedCIReferencedDocumentField;
            }
            set
            {
                this.associatedCIReferencedDocumentField = value;
            }
        }

        public CITransportCargoInsuranceType ApplicableCITransportCargoInsurance
        {
            get
            {
                return this.applicableCITransportCargoInsuranceField;
            }
            set
            {
                this.applicableCITransportCargoInsuranceField = value;
            }
        }

        public CICrossBorderCustomsValuationType ApplicableCICrossBorderCustomsValuation
        {
            get
            {
                return this.applicableCICrossBorderCustomsValuationField;
            }
            set
            {
                this.applicableCICrossBorderCustomsValuationField = value;
            }
        }

        [XmlElement("ApplicableCICrossBorderRegulatoryProcedure")]
        public CICrossBorderRegulatoryProcedureType[] ApplicableCICrossBorderRegulatoryProcedure
        {
            get
            {
                return this.applicableCICrossBorderRegulatoryProcedureField;
            }
            set
            {
                this.applicableCICrossBorderRegulatoryProcedureField = value;
            }
        }

        public CILogisticsLocationType TransitCILogisticsLocation
        {
            get
            {
                return this.transitCILogisticsLocationField;
            }
            set
            {
                this.transitCILogisticsLocationField = value;
            }
        }
    }
}