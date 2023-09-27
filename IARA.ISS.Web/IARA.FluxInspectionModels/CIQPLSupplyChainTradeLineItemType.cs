namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQPLSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQPLSupplyChainTradeLineItemType
    {

        private CIQDocumentLineDocumentType associatedCIQDocumentLineDocumentField;

        private CIQLSupplyChainTradeAgreementType specifiedCIQLSupplyChainTradeAgreementField;

        private CIQLSupplyChainTradeDeliveryType specifiedCIQLSupplyChainTradeDeliveryField;

        private CIQLSupplyChainTradeSettlementType specifiedCIQLSupplyChainTradeSettlementField;

        private CITradeProductType specifiedCITradeProductField;

        private CIReferencedProductType substitutedCIReferencedProductField;

        private ReferencedLogisticsPackageType physicalReferencedLogisticsPackageField;

        public CIQDocumentLineDocumentType AssociatedCIQDocumentLineDocument
        {
            get => this.associatedCIQDocumentLineDocumentField;
            set => this.associatedCIQDocumentLineDocumentField = value;
        }

        public CIQLSupplyChainTradeAgreementType SpecifiedCIQLSupplyChainTradeAgreement
        {
            get => this.specifiedCIQLSupplyChainTradeAgreementField;
            set => this.specifiedCIQLSupplyChainTradeAgreementField = value;
        }

        public CIQLSupplyChainTradeDeliveryType SpecifiedCIQLSupplyChainTradeDelivery
        {
            get => this.specifiedCIQLSupplyChainTradeDeliveryField;
            set => this.specifiedCIQLSupplyChainTradeDeliveryField = value;
        }

        public CIQLSupplyChainTradeSettlementType SpecifiedCIQLSupplyChainTradeSettlement
        {
            get => this.specifiedCIQLSupplyChainTradeSettlementField;
            set => this.specifiedCIQLSupplyChainTradeSettlementField = value;
        }

        public CITradeProductType SpecifiedCITradeProduct
        {
            get => this.specifiedCITradeProductField;
            set => this.specifiedCITradeProductField = value;
        }

        public CIReferencedProductType SubstitutedCIReferencedProduct
        {
            get => this.substitutedCIReferencedProductField;
            set => this.substitutedCIReferencedProductField = value;
        }

        public ReferencedLogisticsPackageType PhysicalReferencedLogisticsPackage
        {
            get => this.physicalReferencedLogisticsPackageField;
            set => this.physicalReferencedLogisticsPackageField = value;
        }
    }
}