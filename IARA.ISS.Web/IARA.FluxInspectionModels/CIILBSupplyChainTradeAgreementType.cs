namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILBSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILBSupplyChainTradeAgreementType
    {

        private CIReferencedDocumentType sellerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType buyerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CITradePriceType[] grossPriceProductCITradePriceField;

        private CITradePriceType[] netPriceProductCITradePriceField;

        public CIReferencedDocumentType SellerOrderReferencedCIReferencedDocument
        {
            get => this.sellerOrderReferencedCIReferencedDocumentField;
            set => this.sellerOrderReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType BuyerOrderReferencedCIReferencedDocument
        {
            get => this.buyerOrderReferencedCIReferencedDocumentField;
            set => this.buyerOrderReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("GrossPriceProductCITradePrice")]
        public CITradePriceType[] GrossPriceProductCITradePrice
        {
            get => this.grossPriceProductCITradePriceField;
            set => this.grossPriceProductCITradePriceField = value;
        }

        [XmlElement("NetPriceProductCITradePrice")]
        public CITradePriceType[] NetPriceProductCITradePrice
        {
            get => this.netPriceProductCITradePriceField;
            set => this.netPriceProductCITradePriceField = value;
        }
    }
}