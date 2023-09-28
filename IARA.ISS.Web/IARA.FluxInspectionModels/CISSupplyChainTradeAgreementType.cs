namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISSupplyChainTradeAgreementType
    {

        private PriorityDescriptionCodeType deliveryPriorityCodeField;

        private TextType[] buyerReferenceField;

        private TextType[] referenceField;

        private QuantityType economicOrderQuantityField;

        private CodeType impactCodeField;

        private QuantityType incrementalProductOrderableQuantityField;

        private QuantityType minimumProductOrderableQuantityField;

        private CodeType priorityCodeField;

        private TextType[] sellerReferenceField;

        private IDType idField;

        private IDType revisionIDField;

        private DateTimeType buyerApprovedDateTimeField;

        private CITradePartyType buyerCITradePartyField;

        private CITradePartyType productEndUserCITradePartyField;

        private CITradePartyType sellerCITradePartyField;

        private CITradeDeliveryTermsType applicableCITradeDeliveryTermsField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] demandForecastReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType priceListReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] requisitionerReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] salesConditionsReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] supplyInstructionReferencedCIReferencedDocumentField;

        private CISupplyChainForecastTermsType applicableCISupplyChainForecastTermsField;

        private CITradePriceType[] grossPriceProductCITradePriceField;

        private CITradePriceType[] netPriceProductCITradePriceField;

        private CITradePartyType primeContractSellerCITradePartyField;

        public PriorityDescriptionCodeType DeliveryPriorityCode
        {
            get => this.deliveryPriorityCodeField;
            set => this.deliveryPriorityCodeField = value;
        }

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

        public QuantityType EconomicOrderQuantity
        {
            get => this.economicOrderQuantityField;
            set => this.economicOrderQuantityField = value;
        }

        public CodeType ImpactCode
        {
            get => this.impactCodeField;
            set => this.impactCodeField = value;
        }

        public QuantityType IncrementalProductOrderableQuantity
        {
            get => this.incrementalProductOrderableQuantityField;
            set => this.incrementalProductOrderableQuantityField = value;
        }

        public QuantityType MinimumProductOrderableQuantity
        {
            get => this.minimumProductOrderableQuantityField;
            set => this.minimumProductOrderableQuantityField = value;
        }

        public CodeType PriorityCode
        {
            get => this.priorityCodeField;
            set => this.priorityCodeField = value;
        }

        [XmlElement("SellerReference")]
        public TextType[] SellerReference
        {
            get => this.sellerReferenceField;
            set => this.sellerReferenceField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType RevisionID
        {
            get => this.revisionIDField;
            set => this.revisionIDField = value;
        }

        public DateTimeType BuyerApprovedDateTime
        {
            get => this.buyerApprovedDateTimeField;
            set => this.buyerApprovedDateTimeField = value;
        }

        public CITradePartyType BuyerCITradeParty
        {
            get => this.buyerCITradePartyField;
            set => this.buyerCITradePartyField = value;
        }

        public CITradePartyType ProductEndUserCITradeParty
        {
            get => this.productEndUserCITradePartyField;
            set => this.productEndUserCITradePartyField = value;
        }

        public CITradePartyType SellerCITradeParty
        {
            get => this.sellerCITradePartyField;
            set => this.sellerCITradePartyField = value;
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

        [XmlElement("DemandForecastReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] DemandForecastReferencedCIReferencedDocument
        {
            get => this.demandForecastReferencedCIReferencedDocumentField;
            set => this.demandForecastReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType PriceListReferencedCIReferencedDocument
        {
            get => this.priceListReferencedCIReferencedDocumentField;
            set => this.priceListReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("RequisitionerReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] RequisitionerReferencedCIReferencedDocument
        {
            get => this.requisitionerReferencedCIReferencedDocumentField;
            set => this.requisitionerReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("SalesConditionsReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] SalesConditionsReferencedCIReferencedDocument
        {
            get => this.salesConditionsReferencedCIReferencedDocumentField;
            set => this.salesConditionsReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("SupplyInstructionReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] SupplyInstructionReferencedCIReferencedDocument
        {
            get => this.supplyInstructionReferencedCIReferencedDocumentField;
            set => this.supplyInstructionReferencedCIReferencedDocumentField = value;
        }

        public CISupplyChainForecastTermsType ApplicableCISupplyChainForecastTerms
        {
            get => this.applicableCISupplyChainForecastTermsField;
            set => this.applicableCISupplyChainForecastTermsField = value;
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

        public CITradePartyType PrimeContractSellerCITradeParty
        {
            get => this.primeContractSellerCITradePartyField;
            set => this.primeContractSellerCITradePartyField = value;
        }
    }
}