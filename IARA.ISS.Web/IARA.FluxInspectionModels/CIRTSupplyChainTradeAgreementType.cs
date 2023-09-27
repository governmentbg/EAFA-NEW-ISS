namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRTSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRTSupplyChainTradeAgreementType
    {

        private CITradePartyType buyerCITradePartyField;

        private CITradePartyType sellerCITradePartyField;

        private CIReferencedDocumentType buyerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType sellerOrderReferencedCIReferencedDocumentField;

        private CITradeDeliveryTermsType applicableCITradeDeliveryTermsField;

        private CITradePriceType agreedPriceProductCITradePriceField;

        public CITradePartyType BuyerCITradeParty
        {
            get => this.buyerCITradePartyField;
            set => this.buyerCITradePartyField = value;
        }

        public CITradePartyType SellerCITradeParty
        {
            get => this.sellerCITradePartyField;
            set => this.sellerCITradePartyField = value;
        }

        public CIReferencedDocumentType BuyerOrderReferencedCIReferencedDocument
        {
            get => this.buyerOrderReferencedCIReferencedDocumentField;
            set => this.buyerOrderReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType SellerOrderReferencedCIReferencedDocument
        {
            get => this.sellerOrderReferencedCIReferencedDocumentField;
            set => this.sellerOrderReferencedCIReferencedDocumentField = value;
        }

        public CITradeDeliveryTermsType ApplicableCITradeDeliveryTerms
        {
            get => this.applicableCITradeDeliveryTermsField;
            set => this.applicableCITradeDeliveryTermsField = value;
        }

        public CITradePriceType AgreedPriceProductCITradePrice
        {
            get => this.agreedPriceProductCITradePriceField;
            set => this.agreedPriceProductCITradePriceField = value;
        }
    }
}