namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDLSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDLSupplyChainTradeAgreementType
    {

        private CIReferencedDocumentType sellerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType buyerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType promotionalDealReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CITradePriceType netPriceProductCITradePriceField;

        private CITradePartyType[] relevantCITradePartyField;

        public CIReferencedDocumentType SellerOrderReferencedCIReferencedDocument
        {
            get
            {
                return this.sellerOrderReferencedCIReferencedDocumentField;
            }
            set
            {
                this.sellerOrderReferencedCIReferencedDocumentField = value;
            }
        }

        public CIReferencedDocumentType BuyerOrderReferencedCIReferencedDocument
        {
            get
            {
                return this.buyerOrderReferencedCIReferencedDocumentField;
            }
            set
            {
                this.buyerOrderReferencedCIReferencedDocumentField = value;
            }
        }

        public CIReferencedDocumentType PromotionalDealReferencedCIReferencedDocument
        {
            get
            {
                return this.promotionalDealReferencedCIReferencedDocumentField;
            }
            set
            {
                this.promotionalDealReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("ContractReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] ContractReferencedCIReferencedDocument
        {
            get
            {
                return this.contractReferencedCIReferencedDocumentField;
            }
            set
            {
                this.contractReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get
            {
                return this.additionalReferencedCIReferencedDocumentField;
            }
            set
            {
                this.additionalReferencedCIReferencedDocumentField = value;
            }
        }

        public CITradePriceType NetPriceProductCITradePrice
        {
            get
            {
                return this.netPriceProductCITradePriceField;
            }
            set
            {
                this.netPriceProductCITradePriceField = value;
            }
        }

        [XmlElement("RelevantCITradeParty")]
        public CITradePartyType[] RelevantCITradeParty
        {
            get
            {
                return this.relevantCITradePartyField;
            }
            set
            {
                this.relevantCITradePartyField = value;
            }
        }
    }
}