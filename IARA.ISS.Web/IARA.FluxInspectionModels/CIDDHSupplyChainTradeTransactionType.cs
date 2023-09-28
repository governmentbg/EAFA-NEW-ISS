namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDHSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDHSupplyChainTradeTransactionType
    {

        private IDType[] shipmentIDField;

        private CIDDHSupplyChainTradeAgreementType applicableCIDDHSupplyChainTradeAgreementField;

        private CIDDHSupplyChainTradeDeliveryType applicableCIDDHSupplyChainTradeDeliveryField;

        private CIDDHSupplyChainTradeSettlementType applicableCIDDHSupplyChainTradeSettlementField;

        private CIDDLSupplyChainTradeLineItemType[] includedCIDDLSupplyChainTradeLineItemField;

        private CIDDLLogisticsPackageType[] specifiedCIDDLLogisticsPackageField;

        [XmlElement("ShipmentID")]
        public IDType[] ShipmentID
        {
            get
            {
                return this.shipmentIDField;
            }
            set
            {
                this.shipmentIDField = value;
            }
        }

        public CIDDHSupplyChainTradeAgreementType ApplicableCIDDHSupplyChainTradeAgreement
        {
            get
            {
                return this.applicableCIDDHSupplyChainTradeAgreementField;
            }
            set
            {
                this.applicableCIDDHSupplyChainTradeAgreementField = value;
            }
        }

        public CIDDHSupplyChainTradeDeliveryType ApplicableCIDDHSupplyChainTradeDelivery
        {
            get
            {
                return this.applicableCIDDHSupplyChainTradeDeliveryField;
            }
            set
            {
                this.applicableCIDDHSupplyChainTradeDeliveryField = value;
            }
        }

        public CIDDHSupplyChainTradeSettlementType ApplicableCIDDHSupplyChainTradeSettlement
        {
            get
            {
                return this.applicableCIDDHSupplyChainTradeSettlementField;
            }
            set
            {
                this.applicableCIDDHSupplyChainTradeSettlementField = value;
            }
        }

        [XmlElement("IncludedCIDDLSupplyChainTradeLineItem")]
        public CIDDLSupplyChainTradeLineItemType[] IncludedCIDDLSupplyChainTradeLineItem
        {
            get
            {
                return this.includedCIDDLSupplyChainTradeLineItemField;
            }
            set
            {
                this.includedCIDDLSupplyChainTradeLineItemField = value;
            }
        }

        [XmlElement("SpecifiedCIDDLLogisticsPackage")]
        public CIDDLLogisticsPackageType[] SpecifiedCIDDLLogisticsPackage
        {
            get
            {
                return this.specifiedCIDDLLogisticsPackageField;
            }
            set
            {
                this.specifiedCIDDLLogisticsPackageField = value;
            }
        }
    }
}