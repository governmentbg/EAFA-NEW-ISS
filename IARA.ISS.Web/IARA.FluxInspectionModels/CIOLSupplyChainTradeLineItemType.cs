namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOLSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOLSupplyChainTradeLineItemType
    {

        private CIOLDocumentLineDocumentType associatedCIOLDocumentLineDocumentField;

        private CIOLSupplyChainTradeAgreementType specifiedCIOLSupplyChainTradeAgreementField;

        private CIOLSupplyChainTradeDeliveryType specifiedCIOLSupplyChainTradeDeliveryField;

        private CIOLSupplyChainTradeSettlementType specifiedCIOLSupplyChainTradeSettlementField;

        private CITradeProductType specifiedCITradeProductField;

        private CIReferencedProductType substitutedCIReferencedProductField;

        private ReferencedLogisticsPackageType physicalReferencedLogisticsPackageField;

        private GoodsProductionType specifiedGoodsProductionField;

        public CIOLDocumentLineDocumentType AssociatedCIOLDocumentLineDocument
        {
            get => this.associatedCIOLDocumentLineDocumentField;
            set => this.associatedCIOLDocumentLineDocumentField = value;
        }

        public CIOLSupplyChainTradeAgreementType SpecifiedCIOLSupplyChainTradeAgreement
        {
            get => this.specifiedCIOLSupplyChainTradeAgreementField;
            set => this.specifiedCIOLSupplyChainTradeAgreementField = value;
        }

        public CIOLSupplyChainTradeDeliveryType SpecifiedCIOLSupplyChainTradeDelivery
        {
            get => this.specifiedCIOLSupplyChainTradeDeliveryField;
            set => this.specifiedCIOLSupplyChainTradeDeliveryField = value;
        }

        public CIOLSupplyChainTradeSettlementType SpecifiedCIOLSupplyChainTradeSettlement
        {
            get => this.specifiedCIOLSupplyChainTradeSettlementField;
            set => this.specifiedCIOLSupplyChainTradeSettlementField = value;
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

        public GoodsProductionType SpecifiedGoodsProduction
        {
            get => this.specifiedGoodsProductionField;
            set => this.specifiedGoodsProductionField = value;
        }
    }
}