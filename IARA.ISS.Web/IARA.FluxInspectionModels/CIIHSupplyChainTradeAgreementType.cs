namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIIHSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIIHSupplyChainTradeAgreementType
    {

        private TextType buyerReferenceField;

        private CITradePartyType sellerCITradePartyField;

        private CITradePartyType buyerCITradePartyField;

        private CITradePartyType buyerAssignedAccountantCITradePartyField;

        private CITradePartyType sellerAssignedAccountantCITradePartyField;

        private CITradePartyType buyerTaxRepresentativeCITradePartyField;

        private CITradePartyType sellerTaxRepresentativeCITradePartyField;

        private CITradeDeliveryTermsType applicableCITradeDeliveryTermsField;

        private CIReferencedDocumentType sellerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType buyerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType quotationReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType orderResponseReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType demandForecastReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType supplyInstructionReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType promotionalDealReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType priceListReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CITradePartyType[] buyerRequisitionerCITradePartyField;

        private CIReferencedDocumentType[] requisitionerReferencedCIReferencedDocumentField;

        private CITradePartyType buyerAgentCITradePartyField;

        private CITradePartyType salesAgentCITradePartyField;

        private ProcuringProjectType specifiedProcuringProjectField;

        private CILogisticsLocationType pricingBaseApplicableCILogisticsLocationField;

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

        public CITradePartyType SellerAssignedAccountantCITradeParty
        {
            get => this.sellerAssignedAccountantCITradePartyField;
            set => this.sellerAssignedAccountantCITradePartyField = value;
        }

        public CITradePartyType BuyerTaxRepresentativeCITradeParty
        {
            get => this.buyerTaxRepresentativeCITradePartyField;
            set => this.buyerTaxRepresentativeCITradePartyField = value;
        }

        public CITradePartyType SellerTaxRepresentativeCITradeParty
        {
            get => this.sellerTaxRepresentativeCITradePartyField;
            set => this.sellerTaxRepresentativeCITradePartyField = value;
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

        public CIReferencedDocumentType OrderResponseReferencedCIReferencedDocument
        {
            get => this.orderResponseReferencedCIReferencedDocumentField;
            set => this.orderResponseReferencedCIReferencedDocumentField = value;
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

        public CIReferencedDocumentType SupplyInstructionReferencedCIReferencedDocument
        {
            get => this.supplyInstructionReferencedCIReferencedDocumentField;
            set => this.supplyInstructionReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType PromotionalDealReferencedCIReferencedDocument
        {
            get => this.promotionalDealReferencedCIReferencedDocumentField;
            set => this.promotionalDealReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType PriceListReferencedCIReferencedDocument
        {
            get => this.priceListReferencedCIReferencedDocumentField;
            set => this.priceListReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
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

        public CITradePartyType BuyerAgentCITradeParty
        {
            get => this.buyerAgentCITradePartyField;
            set => this.buyerAgentCITradePartyField = value;
        }

        public CITradePartyType SalesAgentCITradeParty
        {
            get => this.salesAgentCITradePartyField;
            set => this.salesAgentCITradePartyField = value;
        }

        public ProcuringProjectType SpecifiedProcuringProject
        {
            get => this.specifiedProcuringProjectField;
            set => this.specifiedProcuringProjectField = value;
        }

        public CILogisticsLocationType PricingBaseApplicableCILogisticsLocation
        {
            get => this.pricingBaseApplicableCILogisticsLocationField;
            set => this.pricingBaseApplicableCILogisticsLocationField = value;
        }
    }
}