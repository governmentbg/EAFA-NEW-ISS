namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICHSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICHSupplyChainTradeAgreementType
    {

        private CITradePartyType catalogueInformationReceiverCITradePartyField;

        private CITradePartyType catalogueInformationProviderCITradePartyField;

        private CITradePartyType sellerCITradePartyField;

        private CITradePartyType procurementCITradePartyField;

        private CITradePaymentTermsType applicableCITradePaymentTermsField;

        private CIReferencedDocumentType[] contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType priceListReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType catalogueReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] purchaseConditionsReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] salesConditionsReferencedCIReferencedDocumentField;

        private CITradeCountryType[] targetMarketCITradeCountryField;

        private CILogisticsLocationType[] applicableCILogisticsLocationField;

        private CISpecifiedPeriodType shippingCISpecifiedPeriodField;

        private CIReferencedDocumentType[] catalogueRequestReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] catalogueSubscriptionReferencedCIReferencedDocumentField;

        public CITradePartyType CatalogueInformationReceiverCITradeParty
        {
            get
            {
                return this.catalogueInformationReceiverCITradePartyField;
            }
            set
            {
                this.catalogueInformationReceiverCITradePartyField = value;
            }
        }

        public CITradePartyType CatalogueInformationProviderCITradeParty
        {
            get
            {
                return this.catalogueInformationProviderCITradePartyField;
            }
            set
            {
                this.catalogueInformationProviderCITradePartyField = value;
            }
        }

        public CITradePartyType SellerCITradeParty
        {
            get
            {
                return this.sellerCITradePartyField;
            }
            set
            {
                this.sellerCITradePartyField = value;
            }
        }

        public CITradePartyType ProcurementCITradeParty
        {
            get
            {
                return this.procurementCITradePartyField;
            }
            set
            {
                this.procurementCITradePartyField = value;
            }
        }

        public CITradePaymentTermsType ApplicableCITradePaymentTerms
        {
            get
            {
                return this.applicableCITradePaymentTermsField;
            }
            set
            {
                this.applicableCITradePaymentTermsField = value;
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

        public CIReferencedDocumentType PriceListReferencedCIReferencedDocument
        {
            get
            {
                return this.priceListReferencedCIReferencedDocumentField;
            }
            set
            {
                this.priceListReferencedCIReferencedDocumentField = value;
            }
        }

        public CIReferencedDocumentType CatalogueReferencedCIReferencedDocument
        {
            get
            {
                return this.catalogueReferencedCIReferencedDocumentField;
            }
            set
            {
                this.catalogueReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("PurchaseConditionsReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] PurchaseConditionsReferencedCIReferencedDocument
        {
            get
            {
                return this.purchaseConditionsReferencedCIReferencedDocumentField;
            }
            set
            {
                this.purchaseConditionsReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("SalesConditionsReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] SalesConditionsReferencedCIReferencedDocument
        {
            get
            {
                return this.salesConditionsReferencedCIReferencedDocumentField;
            }
            set
            {
                this.salesConditionsReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("TargetMarketCITradeCountry")]
        public CITradeCountryType[] TargetMarketCITradeCountry
        {
            get
            {
                return this.targetMarketCITradeCountryField;
            }
            set
            {
                this.targetMarketCITradeCountryField = value;
            }
        }

        [XmlElement("ApplicableCILogisticsLocation")]
        public CILogisticsLocationType[] ApplicableCILogisticsLocation
        {
            get
            {
                return this.applicableCILogisticsLocationField;
            }
            set
            {
                this.applicableCILogisticsLocationField = value;
            }
        }

        public CISpecifiedPeriodType ShippingCISpecifiedPeriod
        {
            get
            {
                return this.shippingCISpecifiedPeriodField;
            }
            set
            {
                this.shippingCISpecifiedPeriodField = value;
            }
        }

        [XmlElement("CatalogueRequestReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] CatalogueRequestReferencedCIReferencedDocument
        {
            get
            {
                return this.catalogueRequestReferencedCIReferencedDocumentField;
            }
            set
            {
                this.catalogueRequestReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("CatalogueSubscriptionReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] CatalogueSubscriptionReferencedCIReferencedDocument
        {
            get
            {
                return this.catalogueSubscriptionReferencedCIReferencedDocumentField;
            }
            set
            {
                this.catalogueSubscriptionReferencedCIReferencedDocumentField = value;
            }
        }
    }
}