namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILSupplyChainTradeAgreementType
    {

        private TextType buyerReferenceField;

        private CITradeDeliveryTermsType applicableCITradeDeliveryTermsField;

        private CIReferencedDocumentType sellerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType buyerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType quotationReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType demandForecastReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CITradePriceType grossPriceProductCITradePriceField;

        private CITradePriceType netPriceProductCITradePriceField;

        private CITradePartyType[] buyerRequisitionerCITradePartyField;

        private CIReferencedDocumentType[] requisitionerReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType promotionalDealReferencedCIReferencedDocumentField;

        private CITradePartyType itemSellerCITradePartyField;

        private CITradePartyType itemBuyerCITradePartyField;

        private SpecifiedMarketplaceType includedSpecifiedMarketplaceField;

        public TextType BuyerReference
        {
            get => this.buyerReferenceField;
            set => this.buyerReferenceField = value;
        }

        public CITradeDeliveryTermsType ApplicableCITradeDeliveryTerms
        {
            get => this.applicableCITradeDeliveryTermsField;
            set => this.applicableCITradeDeliveryTermsField = value;
        }

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

        public CIReferencedDocumentType QuotationReferencedCIReferencedDocument
        {
            get => this.quotationReferencedCIReferencedDocumentField;
            set => this.quotationReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType ContractReferencedCIReferencedDocument
        {
            get => this.contractReferencedCIReferencedDocumentField;
            set => this.contractReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType DemandForecastReferencedCIReferencedDocument
        {
            get => this.demandForecastReferencedCIReferencedDocumentField;
            set => this.demandForecastReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
        }

        public CITradePriceType GrossPriceProductCITradePrice
        {
            get => this.grossPriceProductCITradePriceField;
            set => this.grossPriceProductCITradePriceField = value;
        }

        public CITradePriceType NetPriceProductCITradePrice
        {
            get => this.netPriceProductCITradePriceField;
            set => this.netPriceProductCITradePriceField = value;
        }

        [XmlElement("BuyerRequisitionerCITradeParty")]
        public CITradePartyType[] BuyerRequisitionerCITradeParty
        {
            get => this.buyerRequisitionerCITradePartyField;
            set => this.buyerRequisitionerCITradePartyField = value;
        }

        [XmlElement("RequisitionerReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] RequisitionerReferencedCIReferencedDocument
        {
            get => this.requisitionerReferencedCIReferencedDocumentField;
            set => this.requisitionerReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType PromotionalDealReferencedCIReferencedDocument
        {
            get => this.promotionalDealReferencedCIReferencedDocumentField;
            set => this.promotionalDealReferencedCIReferencedDocumentField = value;
        }

        public CITradePartyType ItemSellerCITradeParty
        {
            get => this.itemSellerCITradePartyField;
            set => this.itemSellerCITradePartyField = value;
        }

        public CITradePartyType ItemBuyerCITradeParty
        {
            get => this.itemBuyerCITradePartyField;
            set => this.itemBuyerCITradePartyField = value;
        }

        public SpecifiedMarketplaceType IncludedSpecifiedMarketplace
        {
            get => this.includedSpecifiedMarketplaceField;
            set => this.includedSpecifiedMarketplaceField = value;
        }
    }
}