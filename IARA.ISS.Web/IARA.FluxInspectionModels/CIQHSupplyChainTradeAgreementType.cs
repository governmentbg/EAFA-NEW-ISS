namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQHSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQHSupplyChainTradeAgreementType
    {

        private TextType[] buyerReferenceField;

        private TextType[] referenceField;

        private CITradePartyType buyerCITradePartyField;

        private CITradePartyType sellerCITradePartyField;

        private CIWorkflowObjectType quoteReferencedCIWorkflowObjectField;

        private CITradeDeliveryTermsType applicableCITradeDeliveryTermsField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType priceListReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType quotationProposalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType quotationProposalResponseReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType requestForQuotationReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType requestForQuotationResponseReferencedCIReferencedDocumentField;

        private CITradePartyType[] relevantCITradePartyField;

        private ProcuringProjectType specifiedProcuringProjectField;

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

        public CIWorkflowObjectType QuoteReferencedCIWorkflowObject
        {
            get => this.quoteReferencedCIWorkflowObjectField;
            set => this.quoteReferencedCIWorkflowObjectField = value;
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

        [XmlElement("RelevantCITradeParty")]
        public CITradePartyType[] RelevantCITradeParty
        {
            get => this.relevantCITradePartyField;
            set => this.relevantCITradePartyField = value;
        }

        public ProcuringProjectType SpecifiedProcuringProject
        {
            get => this.specifiedProcuringProjectField;
            set => this.specifiedProcuringProjectField = value;
        }
    }
}