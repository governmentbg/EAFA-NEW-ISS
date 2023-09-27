namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOHSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOHSupplyChainTradeAgreementType
    {

        private TextType buyerReferenceField;

        private CITradePartyType sellerCITradePartyField;

        private CITradePartyType buyerCITradePartyField;

        private CITradePartyType buyerAssignedAccountantCITradePartyField;

        private CIReferencedDocumentType sellerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType buyerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType marketplaceOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType quotationReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType requisitionReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType priceListReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType requestForQuotationReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType salesReportReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType blanketOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType previousOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType previousOrderChangeReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType previousOrderResponseReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType originalOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType engineeringChangeReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType letterOfCreditReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType importLicenceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType exportLicenceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CITradePartyType[] relevantCITradePartyField;

        private CITradePartyType buyerRequisitionerCITradePartyField;

        private ProcuringProjectType specifiedProcuringProjectField;

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

        public CITradePartyType BuyerCITradeParty
        {
            get => this.buyerCITradePartyField;
            set => this.buyerCITradePartyField = value;
        }

        public CITradePartyType BuyerAssignedAccountantCITradeParty
        {
            get => this.buyerAssignedAccountantCITradePartyField;
            set => this.buyerAssignedAccountantCITradePartyField = value;
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

        [XmlElement("ContractReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] ContractReferencedCIReferencedDocument
        {
            get => this.contractReferencedCIReferencedDocumentField;
            set => this.contractReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType RequisitionReferencedCIReferencedDocument
        {
            get => this.requisitionReferencedCIReferencedDocumentField;
            set => this.requisitionReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType PriceListReferencedCIReferencedDocument
        {
            get => this.priceListReferencedCIReferencedDocumentField;
            set => this.priceListReferencedCIReferencedDocumentField = value;
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

        public CIReferencedDocumentType PreviousOrderChangeReferencedCIReferencedDocument
        {
            get => this.previousOrderChangeReferencedCIReferencedDocumentField;
            set => this.previousOrderChangeReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType PreviousOrderResponseReferencedCIReferencedDocument
        {
            get => this.previousOrderResponseReferencedCIReferencedDocumentField;
            set => this.previousOrderResponseReferencedCIReferencedDocumentField = value;
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

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("RelevantCITradeParty")]
        public CITradePartyType[] RelevantCITradeParty
        {
            get => this.relevantCITradePartyField;
            set => this.relevantCITradePartyField = value;
        }

        public CITradePartyType BuyerRequisitionerCITradeParty
        {
            get => this.buyerRequisitionerCITradePartyField;
            set => this.buyerRequisitionerCITradePartyField = value;
        }

        public ProcuringProjectType SpecifiedProcuringProject
        {
            get => this.specifiedProcuringProjectField;
            set => this.specifiedProcuringProjectField = value;
        }
    }
}