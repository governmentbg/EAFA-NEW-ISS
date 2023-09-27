namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICLSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICLSupplyChainTradeAgreementType
    {

        private CodeType productAvailabilityCodeField;

        private IndicatorType productOrderableIndicatorField;

        private IndicatorType productReorderableIndicatorField;

        private IndicatorType productMadeToOrderIndicatorField;

        private DurationUnitMeasureType deliveryOrderFulfilmentLeadTimeMeasureField;

        private DurationUnitMeasureType pickUpOrderFulfilmentLeadTimeMeasureField;

        private QuantityType minimumProductOrderableQuantityField;

        private QuantityType maximumProductOrderableQuantityField;

        private QuantityType incrementalProductOrderableQuantityField;

        private CodeType orderProductUnitMeasureCodeField;

        private CodeType resaleProductUnitMeasureCodeField;

        private IndicatorType informationUseRestrictionIndicatorField;

        private CITradePartyType sellerCITradePartyField;

        private CITradePartyType procurementCITradePartyField;

        private CITradePartyType catalogueInformationProviderCITradePartyField;

        private CITradePartyType supportCentreCITradePartyField;

        private CITradePartyType carrierCITradePartyField;

        private CITradeDeliveryTermsType applicableCITradeDeliveryTermsField;

        private CITradePriceType[] grossPriceProductCITradePriceField;

        private CITradePriceType[] netPriceProductCITradePriceField;

        private CITradeCountryType[] targetMarketCITradeCountryField;

        private CISpecifiedPeriodType orderingCISpecifiedPeriodField;

        private CISpecifiedPeriodType minimumOrderQuantityOrderingCISpecifiedPeriodField;

        private CISpecifiedPeriodType maximumOrderQuantityOrderingCISpecifiedPeriodField;

        private CISpecifiedPeriodType exclusivityCISpecifiedPeriodField;

        private CISpecifiedPeriodType resaleCISpecifiedPeriodField;

        private CISpecifiedPeriodType guaranteedProductLifeSpanCISpecifiedPeriodField;

        private CIReferencedDocumentType immediatePreviousPriceListReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType priceListReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] salesConditionsReferencedCIReferencedDocumentField;

        public CodeType ProductAvailabilityCode
        {
            get
            {
                return this.productAvailabilityCodeField;
            }
            set
            {
                this.productAvailabilityCodeField = value;
            }
        }

        public IndicatorType ProductOrderableIndicator
        {
            get
            {
                return this.productOrderableIndicatorField;
            }
            set
            {
                this.productOrderableIndicatorField = value;
            }
        }

        public IndicatorType ProductReorderableIndicator
        {
            get
            {
                return this.productReorderableIndicatorField;
            }
            set
            {
                this.productReorderableIndicatorField = value;
            }
        }

        public IndicatorType ProductMadeToOrderIndicator
        {
            get
            {
                return this.productMadeToOrderIndicatorField;
            }
            set
            {
                this.productMadeToOrderIndicatorField = value;
            }
        }

        public DurationUnitMeasureType DeliveryOrderFulfilmentLeadTimeMeasure
        {
            get
            {
                return this.deliveryOrderFulfilmentLeadTimeMeasureField;
            }
            set
            {
                this.deliveryOrderFulfilmentLeadTimeMeasureField = value;
            }
        }

        public DurationUnitMeasureType PickUpOrderFulfilmentLeadTimeMeasure
        {
            get
            {
                return this.pickUpOrderFulfilmentLeadTimeMeasureField;
            }
            set
            {
                this.pickUpOrderFulfilmentLeadTimeMeasureField = value;
            }
        }

        public QuantityType MinimumProductOrderableQuantity
        {
            get
            {
                return this.minimumProductOrderableQuantityField;
            }
            set
            {
                this.minimumProductOrderableQuantityField = value;
            }
        }

        public QuantityType MaximumProductOrderableQuantity
        {
            get
            {
                return this.maximumProductOrderableQuantityField;
            }
            set
            {
                this.maximumProductOrderableQuantityField = value;
            }
        }

        public QuantityType IncrementalProductOrderableQuantity
        {
            get
            {
                return this.incrementalProductOrderableQuantityField;
            }
            set
            {
                this.incrementalProductOrderableQuantityField = value;
            }
        }

        public CodeType OrderProductUnitMeasureCode
        {
            get
            {
                return this.orderProductUnitMeasureCodeField;
            }
            set
            {
                this.orderProductUnitMeasureCodeField = value;
            }
        }

        public CodeType ResaleProductUnitMeasureCode
        {
            get
            {
                return this.resaleProductUnitMeasureCodeField;
            }
            set
            {
                this.resaleProductUnitMeasureCodeField = value;
            }
        }

        public IndicatorType InformationUseRestrictionIndicator
        {
            get
            {
                return this.informationUseRestrictionIndicatorField;
            }
            set
            {
                this.informationUseRestrictionIndicatorField = value;
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

        public CITradePartyType SupportCentreCITradeParty
        {
            get
            {
                return this.supportCentreCITradePartyField;
            }
            set
            {
                this.supportCentreCITradePartyField = value;
            }
        }

        public CITradePartyType CarrierCITradeParty
        {
            get
            {
                return this.carrierCITradePartyField;
            }
            set
            {
                this.carrierCITradePartyField = value;
            }
        }

        public CITradeDeliveryTermsType ApplicableCITradeDeliveryTerms
        {
            get
            {
                return this.applicableCITradeDeliveryTermsField;
            }
            set
            {
                this.applicableCITradeDeliveryTermsField = value;
            }
        }

        [XmlElement("GrossPriceProductCITradePrice")]
        public CITradePriceType[] GrossPriceProductCITradePrice
        {
            get
            {
                return this.grossPriceProductCITradePriceField;
            }
            set
            {
                this.grossPriceProductCITradePriceField = value;
            }
        }

        [XmlElement("NetPriceProductCITradePrice")]
        public CITradePriceType[] NetPriceProductCITradePrice
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

        public CISpecifiedPeriodType OrderingCISpecifiedPeriod
        {
            get
            {
                return this.orderingCISpecifiedPeriodField;
            }
            set
            {
                this.orderingCISpecifiedPeriodField = value;
            }
        }

        public CISpecifiedPeriodType MinimumOrderQuantityOrderingCISpecifiedPeriod
        {
            get
            {
                return this.minimumOrderQuantityOrderingCISpecifiedPeriodField;
            }
            set
            {
                this.minimumOrderQuantityOrderingCISpecifiedPeriodField = value;
            }
        }

        public CISpecifiedPeriodType MaximumOrderQuantityOrderingCISpecifiedPeriod
        {
            get
            {
                return this.maximumOrderQuantityOrderingCISpecifiedPeriodField;
            }
            set
            {
                this.maximumOrderQuantityOrderingCISpecifiedPeriodField = value;
            }
        }

        public CISpecifiedPeriodType ExclusivityCISpecifiedPeriod
        {
            get
            {
                return this.exclusivityCISpecifiedPeriodField;
            }
            set
            {
                this.exclusivityCISpecifiedPeriodField = value;
            }
        }

        public CISpecifiedPeriodType ResaleCISpecifiedPeriod
        {
            get
            {
                return this.resaleCISpecifiedPeriodField;
            }
            set
            {
                this.resaleCISpecifiedPeriodField = value;
            }
        }

        public CISpecifiedPeriodType GuaranteedProductLifeSpanCISpecifiedPeriod
        {
            get
            {
                return this.guaranteedProductLifeSpanCISpecifiedPeriodField;
            }
            set
            {
                this.guaranteedProductLifeSpanCISpecifiedPeriodField = value;
            }
        }

        public CIReferencedDocumentType ImmediatePreviousPriceListReferencedCIReferencedDocument
        {
            get
            {
                return this.immediatePreviousPriceListReferencedCIReferencedDocumentField;
            }
            set
            {
                this.immediatePreviousPriceListReferencedCIReferencedDocumentField = value;
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
    }
}