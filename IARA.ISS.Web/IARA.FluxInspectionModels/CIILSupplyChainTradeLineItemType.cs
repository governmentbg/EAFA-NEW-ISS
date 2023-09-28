namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILSupplyChainTradeLineItemType
    {

        private CIILDocumentLineDocumentType associatedCIILDocumentLineDocumentField;

        private CIILSupplyChainTradeAgreementType specifiedCIILSupplyChainTradeAgreementField;

        private CIILSupplyChainTradeDeliveryType specifiedCIILSupplyChainTradeDeliveryField;

        private CIILSupplyChainTradeSettlementType specifiedCIILSupplyChainTradeSettlementField;

        private CITradeProductType specifiedCITradeProductField;

        private CIILBSubordinateTradeLineItemType[] subordinateCIILBSubordinateTradeLineItemField;

        public CIILDocumentLineDocumentType AssociatedCIILDocumentLineDocument
        {
            get => this.associatedCIILDocumentLineDocumentField;
            set => this.associatedCIILDocumentLineDocumentField = value;
        }

        public CIILSupplyChainTradeAgreementType SpecifiedCIILSupplyChainTradeAgreement
        {
            get => this.specifiedCIILSupplyChainTradeAgreementField;
            set => this.specifiedCIILSupplyChainTradeAgreementField = value;
        }

        public CIILSupplyChainTradeDeliveryType SpecifiedCIILSupplyChainTradeDelivery
        {
            get => this.specifiedCIILSupplyChainTradeDeliveryField;
            set => this.specifiedCIILSupplyChainTradeDeliveryField = value;
        }

        public CIILSupplyChainTradeSettlementType SpecifiedCIILSupplyChainTradeSettlement
        {
            get => this.specifiedCIILSupplyChainTradeSettlementField;
            set => this.specifiedCIILSupplyChainTradeSettlementField = value;
        }

        public CITradeProductType SpecifiedCITradeProduct
        {
            get => this.specifiedCITradeProductField;
            set => this.specifiedCITradeProductField = value;
        }

        [XmlElement("SubordinateCIILBSubordinateTradeLineItem")]
        public CIILBSubordinateTradeLineItemType[] SubordinateCIILBSubordinateTradeLineItem
        {
            get => this.subordinateCIILBSubordinateTradeLineItemField;
            set => this.subordinateCIILBSubordinateTradeLineItemField = value;
        }
    }
}