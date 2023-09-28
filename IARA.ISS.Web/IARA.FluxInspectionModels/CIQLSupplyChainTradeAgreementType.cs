namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQLSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQLSupplyChainTradeAgreementType
    {

        private TextType[] buyerReferenceField;

        private TextType[] referenceField;

        private CITradePartyType productEndUserCITradePartyField;

        private CITradeDeliveryTermsType applicableCITradeDeliveryTermsField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType priceListReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType quotationProposalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType quotationProposalResponseReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType requestForQuotationReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType requestForQuotationResponseReferencedCIReferencedDocumentField;

        private CITradePriceType[] grossPriceProductCITradePriceField;

        private CITradePriceType[] netPriceProductCITradePriceField;

        [XmlElement("BuyerReference")]
        public TextType[] BuyerReference
        {
            get => this.buyerReferenceField;
            set => this.buyerReferenceField = value;
        }

        [XmlElement("Reference")]
        public TextType[] Reference
        {
            get => this.referenceField;
            set => this.referenceField = value;
        }

        public CITradePartyType ProductEndUserCITradeParty
        {
            get => this.productEndUserCITradePartyField;
            set => this.productEndUserCITradePartyField = value;
        }

        public CITradeDeliveryTermsType ApplicableCITradeDeliveryTerms
        {
            get => this.applicableCITradeDeliveryTermsField;
            set => this.applicableCITradeDeliveryTermsField = value;
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("ContractReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] ContractReferencedCIReferencedDocument
        {
            get => this.contractReferencedCIReferencedDocumentField;
            set => this.contractReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType PriceListReferencedCIReferencedDocument
        {
            get => this.priceListReferencedCIReferencedDocumentField;
            set => this.priceListReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType QuotationProposalReferencedCIReferencedDocument
        {
            get => this.quotationProposalReferencedCIReferencedDocumentField;
            set => this.quotationProposalReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType QuotationProposalResponseReferencedCIReferencedDocument
        {
            get => this.quotationProposalResponseReferencedCIReferencedDocumentField;
            set => this.quotationProposalResponseReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType RequestForQuotationReferencedCIReferencedDocument
        {
            get => this.requestForQuotationReferencedCIReferencedDocumentField;
            set => this.requestForQuotationReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType RequestForQuotationResponseReferencedCIReferencedDocument
        {
            get => this.requestForQuotationResponseReferencedCIReferencedDocumentField;
            set => this.requestForQuotationResponseReferencedCIReferencedDocumentField = value;
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