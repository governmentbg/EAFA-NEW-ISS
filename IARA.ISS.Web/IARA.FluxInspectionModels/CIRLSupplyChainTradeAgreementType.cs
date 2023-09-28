namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRLSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRLSupplyChainTradeAgreementType
    {

        private CIReferencedDocumentType buyerOrderReferencedCIReferencedDocumentField;

        private CITradePriceType orderPriceProductCITradePriceField;

        private CIReferencedDocumentType sellerOrderReferencedCIReferencedDocumentField;

        public CIReferencedDocumentType BuyerOrderReferencedCIReferencedDocument
        {
            get => this.buyerOrderReferencedCIReferencedDocumentField;
            set => this.buyerOrderReferencedCIReferencedDocumentField = value;
        }

        public CITradePriceType OrderPriceProductCITradePrice
        {
            get => this.orderPriceProductCITradePriceField;
            set => this.orderPriceProductCITradePriceField = value;
        }

        public CIReferencedDocumentType SellerOrderReferencedCIReferencedDocument
        {
            get => this.sellerOrderReferencedCIReferencedDocumentField;
            set => this.sellerOrderReferencedCIReferencedDocumentField = value;
        }
    }
}