namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOLSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOLSupplyChainTradeAgreementType
    {

        private TextType buyerReferenceField;

        private CITradePartyType sellerCITradePartyField;

        private CITradePartyType buyerRequisitionerCITradePartyField;

        private CIReferencedDocumentType sellerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType buyerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType marketplaceOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType quotationReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType requisitionReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType requestForQuotationReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType salesReportReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType blanketOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType previousOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType originalOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType engineeringChangeReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType letterOfCreditReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType importLicenceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType exportLicenceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType catalogueReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CITradePriceType grossPriceProductCITradePriceField;

        private CITradePriceType netPriceProductCITradePriceField;

        private CITradeDeliveryTermsType applicableCITradeDeliveryTermsField;

        public TextType BuyerReference
        {
            get => this.buyerReferenceField;
            set => this.buyerReferenceField = value;
        }

        public CITradePartyType SellerCITradeParty
        {
            get => this.sellerCITradePartyField;
            set => this.sellerCITradePartyField = value;
        }

        public CITradePartyType BuyerRequisitionerCITradeParty
        {
            get => this.buyerRequisitionerCITradePartyField;
            set => this.buyerRequisitionerCITradePartyField = value;
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

        public CIReferencedDocumentType MarketplaceOrderReferencedCIReferencedDocument
        {
            get => this.marketplaceOrderReferencedCIReferencedDocumentField;
            set => this.marketplaceOrderReferencedCIReferencedDocumentField = value;
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

        public CIReferencedDocumentType RequisitionReferencedCIReferencedDocument
        {
            get => this.requisitionReferencedCIReferencedDocumentField;
            set => this.requisitionReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType RequestForQuotationReferencedCIReferencedDocument
        {
            get => this.requestForQuotationReferencedCIReferencedDocumentField;
            set => this.requestForQuotationReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType SalesReportReferencedCIReferencedDocument
        {
            get => this.salesReportReferencedCIReferencedDocumentField;
            set => this.salesReportReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType BlanketOrderReferencedCIReferencedDocument
        {
            get => this.blanketOrderReferencedCIReferencedDocumentField;
            set => this.blanketOrderReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType PreviousOrderReferencedCIReferencedDocument
        {
            get => this.previousOrderReferencedCIReferencedDocumentField;
            set => this.previousOrderReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType OriginalOrderReferencedCIReferencedDocument
        {
            get => this.originalOrderReferencedCIReferencedDocumentField;
            set => this.originalOrderReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType EngineeringChangeReferencedCIReferencedDocument
        {
            get => this.engineeringChangeReferencedCIReferencedDocumentField;
            set => this.engineeringChangeReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType LetterOfCreditReferencedCIReferencedDocument
        {
            get => this.letterOfCreditReferencedCIReferencedDocumentField;
            set => this.letterOfCreditReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType ImportLicenceReferencedCIReferencedDocument
        {
            get => this.importLicenceReferencedCIReferencedDocumentField;
            set => this.importLicenceReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType ExportLicenceReferencedCIReferencedDocument
        {
            get => this.exportLicenceReferencedCIReferencedDocumentField;
            set => this.exportLicenceReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType CatalogueReferencedCIReferencedDocument
        {
            get => this.catalogueReferencedCIReferencedDocumentField;
            set => this.catalogueReferencedCIReferencedDocumentField = value;
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

        public CITradeDeliveryTermsType ApplicableCITradeDeliveryTerms
        {
            get => this.applicableCITradeDeliveryTermsField;
            set => this.applicableCITradeDeliveryTermsField = value;
        }
    }
}