namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CatalogueTradeProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CatalogueTradeProductType
    {

        private IDType[] idField;

        private IDType[] globalIDField;

        private IDType sellerAssignedIDField;

        private IDType buyerAssignedIDField;

        private IDType[] manufacturerAssignedIDField;

        private IDType[] industryAssignedIDField;

        private IDType[] trackingSystemIDField;

        private TextType[] nameField;

        private MeasureType[] grossWeightMeasureField;

        private MeasureType drainedNetWeightMeasureField;

        private DurationUnitMeasureType fromOpeningLifeSpanMeasureField;

        private DurationUnitMeasureType fromDeliveryLifeSpanMeasureField;

        private MeasureType areaDensityMeasureField;

        private DurationUnitMeasureType fromProductionLifeSpanMeasureField;

        private CodeType[] unitTypeCodeField;

        private DateTimeType productionDiscontinuedDateTimeField;

        private string cancellationAnnouncedLaunchDateTimeField;

        private IDType[] batchIDField;

        private string latestProductDataChangeDateTimeField;

        private TextType[] functionDescriptionField;

        private TextType[] conciseDescriptionField;

        private TextType[] variantDescriptionField;

        private TextType[] additionalDescriptionField;

        private TextType[] physicalFormDescriptionField;

        private TextType brandNameField;

        private TextType subBrandNameField;

        private TextType commonNameField;

        private TextType scientificNameField;

        private IndicatorType contentVariableMeasureIndicatorField;

        private IndicatorType configurableIndicatorField;

        private IndicatorType markedSerialNumberIndicatorField;

        private IndicatorType prePackagedIndicatorField;

        private CodeType colourCodeField;

        private TextType[] colourDescriptionField;

        private QuantityType contentUnitQuantityField;

        private QuantityType innerPackContentUnitQuantityField;

        private QuantityType innerPackQuantityField;

        private QuantityType includedProductContentUnitQuantityField;

        private QuantityType includedProductTypeQuantityField;

        private IDType promotionalVariantIDField;

        private TextType[] consumerAgeDescriptionField;

        private TextType[] consumerGenderDescriptionField;

        private TextType[] marketingDescriptionField;

        private TextType[] seasonDescriptionField;

        private TextType[] sizeDescriptionField;

        private TextType brandRangeNameField;

        private TextType modelNameField;

        private CodeType[] seasonCodeField;

        private IDType[] regulationConformityIDField;

        private CITradePartyType[] manufacturerCITradePartyField;

        private CITradePartyType legalRightsOwnerCITradePartyField;

        private CITradePartyType brandOwnerCITradePartyField;

        private CIProductCharacteristicType[] applicableCIProductCharacteristicField;

        private CIMaterialGoodsCharacteristicType[] applicableCIMaterialGoodsCharacteristicField;

        private CIProductClassificationType[] designatedCIProductClassificationField;

        private CITradeProductInstanceType[] individualCITradeProductInstanceField;

        private CIReferencedProductType[] includedCIReferencedProductField;

        private CITradeCountryType[] originCITradeCountryField;

        private CITradeCountryType[] suppliedFromCITradeCountryField;

        private CITradeCountryType[] finalAssemblyCITradeCountryField;

        private CISpatialDimensionType[] linearCISpatialDimensionField;

        private CISpatialDimensionType[] minimumLinearCISpatialDimensionField;

        private CISpatialDimensionType[] maximumLinearCISpatialDimensionField;

        private TradeProductSupplyChainPackagingType[] applicableTradeProductSupplyChainPackagingField;

        private CIReferencedDocumentType[] marketingCampaignReferenceCIReferencedDocumentField;

        private SpecifiedBinaryFileType[] presentationSpecifiedBinaryFileField;

        private TextType[][] applicableKeywordField;

        private ProductSecurityTagType[] attachedProductSecurityTagField;

        private TradeProductFeatureType[] marketingTradeProductFeatureField;

        private CIReferencedDocumentType[] mSDSReferenceCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferenceCIReferencedDocumentField;

        private CINoteType[] informationCINoteField;

        private TradeProductCertificationType[] applicableTradeProductCertificationField;

        private CIDisposalInstructionsType[] applicableCIDisposalInstructionsField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        [XmlElement("GlobalID")]
        public IDType[] GlobalID
        {
            get
            {
                return this.globalIDField;
            }
            set
            {
                this.globalIDField = value;
            }
        }

        public IDType SellerAssignedID
        {
            get
            {
                return this.sellerAssignedIDField;
            }
            set
            {
                this.sellerAssignedIDField = value;
            }
        }

        public IDType BuyerAssignedID
        {
            get
            {
                return this.buyerAssignedIDField;
            }
            set
            {
                this.buyerAssignedIDField = value;
            }
        }

        [XmlElement("ManufacturerAssignedID")]
        public IDType[] ManufacturerAssignedID
        {
            get
            {
                return this.manufacturerAssignedIDField;
            }
            set
            {
                this.manufacturerAssignedIDField = value;
            }
        }

        [XmlElement("IndustryAssignedID")]
        public IDType[] IndustryAssignedID
        {
            get
            {
                return this.industryAssignedIDField;
            }
            set
            {
                this.industryAssignedIDField = value;
            }
        }

        [XmlElement("TrackingSystemID")]
        public IDType[] TrackingSystemID
        {
            get
            {
                return this.trackingSystemIDField;
            }
            set
            {
                this.trackingSystemIDField = value;
            }
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlElement("GrossWeightMeasure")]
        public MeasureType[] GrossWeightMeasure
        {
            get
            {
                return this.grossWeightMeasureField;
            }
            set
            {
                this.grossWeightMeasureField = value;
            }
        }

        public MeasureType DrainedNetWeightMeasure
        {
            get
            {
                return this.drainedNetWeightMeasureField;
            }
            set
            {
                this.drainedNetWeightMeasureField = value;
            }
        }

        public DurationUnitMeasureType FromOpeningLifeSpanMeasure
        {
            get
            {
                return this.fromOpeningLifeSpanMeasureField;
            }
            set
            {
                this.fromOpeningLifeSpanMeasureField = value;
            }
        }

        public DurationUnitMeasureType FromDeliveryLifeSpanMeasure
        {
            get
            {
                return this.fromDeliveryLifeSpanMeasureField;
            }
            set
            {
                this.fromDeliveryLifeSpanMeasureField = value;
            }
        }

        public MeasureType AreaDensityMeasure
        {
            get
            {
                return this.areaDensityMeasureField;
            }
            set
            {
                this.areaDensityMeasureField = value;
            }
        }

        public DurationUnitMeasureType FromProductionLifeSpanMeasure
        {
            get
            {
                return this.fromProductionLifeSpanMeasureField;
            }
            set
            {
                this.fromProductionLifeSpanMeasureField = value;
            }
        }

        [XmlElement("UnitTypeCode")]
        public CodeType[] UnitTypeCode
        {
            get
            {
                return this.unitTypeCodeField;
            }
            set
            {
                this.unitTypeCodeField = value;
            }
        }

        public DateTimeType ProductionDiscontinuedDateTime
        {
            get
            {
                return this.productionDiscontinuedDateTimeField;
            }
            set
            {
                this.productionDiscontinuedDateTimeField = value;
            }
        }

        public string CancellationAnnouncedLaunchDateTime
        {
            get
            {
                return this.cancellationAnnouncedLaunchDateTimeField;
            }
            set
            {
                this.cancellationAnnouncedLaunchDateTimeField = value;
            }
        }

        [XmlElement("BatchID")]
        public IDType[] BatchID
        {
            get
            {
                return this.batchIDField;
            }
            set
            {
                this.batchIDField = value;
            }
        }

        public string LatestProductDataChangeDateTime
        {
            get
            {
                return this.latestProductDataChangeDateTimeField;
            }
            set
            {
                this.latestProductDataChangeDateTimeField = value;
            }
        }

        [XmlElement("FunctionDescription")]
        public TextType[] FunctionDescription
        {
            get
            {
                return this.functionDescriptionField;
            }
            set
            {
                this.functionDescriptionField = value;
            }
        }

        [XmlElement("ConciseDescription")]
        public TextType[] ConciseDescription
        {
            get
            {
                return this.conciseDescriptionField;
            }
            set
            {
                this.conciseDescriptionField = value;
            }
        }

        [XmlElement("VariantDescription")]
        public TextType[] VariantDescription
        {
            get
            {
                return this.variantDescriptionField;
            }
            set
            {
                this.variantDescriptionField = value;
            }
        }

        [XmlElement("AdditionalDescription")]
        public TextType[] AdditionalDescription
        {
            get
            {
                return this.additionalDescriptionField;
            }
            set
            {
                this.additionalDescriptionField = value;
            }
        }

        [XmlElement("PhysicalFormDescription")]
        public TextType[] PhysicalFormDescription
        {
            get
            {
                return this.physicalFormDescriptionField;
            }
            set
            {
                this.physicalFormDescriptionField = value;
            }
        }

        public TextType BrandName
        {
            get
            {
                return this.brandNameField;
            }
            set
            {
                this.brandNameField = value;
            }
        }

        public TextType SubBrandName
        {
            get
            {
                return this.subBrandNameField;
            }
            set
            {
                this.subBrandNameField = value;
            }
        }

        public TextType CommonName
        {
            get
            {
                return this.commonNameField;
            }
            set
            {
                this.commonNameField = value;
            }
        }

        public TextType ScientificName
        {
            get
            {
                return this.scientificNameField;
            }
            set
            {
                this.scientificNameField = value;
            }
        }

        public IndicatorType ContentVariableMeasureIndicator
        {
            get
            {
                return this.contentVariableMeasureIndicatorField;
            }
            set
            {
                this.contentVariableMeasureIndicatorField = value;
            }
        }

        public IndicatorType ConfigurableIndicator
        {
            get
            {
                return this.configurableIndicatorField;
            }
            set
            {
                this.configurableIndicatorField = value;
            }
        }

        public IndicatorType MarkedSerialNumberIndicator
        {
            get
            {
                return this.markedSerialNumberIndicatorField;
            }
            set
            {
                this.markedSerialNumberIndicatorField = value;
            }
        }

        public IndicatorType PrePackagedIndicator
        {
            get
            {
                return this.prePackagedIndicatorField;
            }
            set
            {
                this.prePackagedIndicatorField = value;
            }
        }

        public CodeType ColourCode
        {
            get
            {
                return this.colourCodeField;
            }
            set
            {
                this.colourCodeField = value;
            }
        }

        [XmlElement("ColourDescription")]
        public TextType[] ColourDescription
        {
            get
            {
                return this.colourDescriptionField;
            }
            set
            {
                this.colourDescriptionField = value;
            }
        }

        public QuantityType ContentUnitQuantity
        {
            get
            {
                return this.contentUnitQuantityField;
            }
            set
            {
                this.contentUnitQuantityField = value;
            }
        }

        public QuantityType InnerPackContentUnitQuantity
        {
            get
            {
                return this.innerPackContentUnitQuantityField;
            }
            set
            {
                this.innerPackContentUnitQuantityField = value;
            }
        }

        public QuantityType InnerPackQuantity
        {
            get
            {
                return this.innerPackQuantityField;
            }
            set
            {
                this.innerPackQuantityField = value;
            }
        }

        public QuantityType IncludedProductContentUnitQuantity
        {
            get
            {
                return this.includedProductContentUnitQuantityField;
            }
            set
            {
                this.includedProductContentUnitQuantityField = value;
            }
        }

        public QuantityType IncludedProductTypeQuantity
        {
            get
            {
                return this.includedProductTypeQuantityField;
            }
            set
            {
                this.includedProductTypeQuantityField = value;
            }
        }

        public IDType PromotionalVariantID
        {
            get
            {
                return this.promotionalVariantIDField;
            }
            set
            {
                this.promotionalVariantIDField = value;
            }
        }

        [XmlElement("ConsumerAgeDescription")]
        public TextType[] ConsumerAgeDescription
        {
            get
            {
                return this.consumerAgeDescriptionField;
            }
            set
            {
                this.consumerAgeDescriptionField = value;
            }
        }

        [XmlElement("ConsumerGenderDescription")]
        public TextType[] ConsumerGenderDescription
        {
            get
            {
                return this.consumerGenderDescriptionField;
            }
            set
            {
                this.consumerGenderDescriptionField = value;
            }
        }

        [XmlElement("MarketingDescription")]
        public TextType[] MarketingDescription
        {
            get
            {
                return this.marketingDescriptionField;
            }
            set
            {
                this.marketingDescriptionField = value;
            }
        }

        [XmlElement("SeasonDescription")]
        public TextType[] SeasonDescription
        {
            get
            {
                return this.seasonDescriptionField;
            }
            set
            {
                this.seasonDescriptionField = value;
            }
        }

        [XmlElement("SizeDescription")]
        public TextType[] SizeDescription
        {
            get
            {
                return this.sizeDescriptionField;
            }
            set
            {
                this.sizeDescriptionField = value;
            }
        }

        public TextType BrandRangeName
        {
            get
            {
                return this.brandRangeNameField;
            }
            set
            {
                this.brandRangeNameField = value;
            }
        }

        public TextType ModelName
        {
            get
            {
                return this.modelNameField;
            }
            set
            {
                this.modelNameField = value;
            }
        }

        [XmlElement("SeasonCode")]
        public CodeType[] SeasonCode
        {
            get
            {
                return this.seasonCodeField;
            }
            set
            {
                this.seasonCodeField = value;
            }
        }

        [XmlElement("RegulationConformityID")]
        public IDType[] RegulationConformityID
        {
            get
            {
                return this.regulationConformityIDField;
            }
            set
            {
                this.regulationConformityIDField = value;
            }
        }

        [XmlElement("ManufacturerCITradeParty")]
        public CITradePartyType[] ManufacturerCITradeParty
        {
            get
            {
                return this.manufacturerCITradePartyField;
            }
            set
            {
                this.manufacturerCITradePartyField = value;
            }
        }

        public CITradePartyType LegalRightsOwnerCITradeParty
        {
            get
            {
                return this.legalRightsOwnerCITradePartyField;
            }
            set
            {
                this.legalRightsOwnerCITradePartyField = value;
            }
        }

        public CITradePartyType BrandOwnerCITradeParty
        {
            get
            {
                return this.brandOwnerCITradePartyField;
            }
            set
            {
                this.brandOwnerCITradePartyField = value;
            }
        }

        [XmlElement("ApplicableCIProductCharacteristic")]
        public CIProductCharacteristicType[] ApplicableCIProductCharacteristic
        {
            get
            {
                return this.applicableCIProductCharacteristicField;
            }
            set
            {
                this.applicableCIProductCharacteristicField = value;
            }
        }

        [XmlElement("ApplicableCIMaterialGoodsCharacteristic")]
        public CIMaterialGoodsCharacteristicType[] ApplicableCIMaterialGoodsCharacteristic
        {
            get
            {
                return this.applicableCIMaterialGoodsCharacteristicField;
            }
            set
            {
                this.applicableCIMaterialGoodsCharacteristicField = value;
            }
        }

        [XmlElement("DesignatedCIProductClassification")]
        public CIProductClassificationType[] DesignatedCIProductClassification
        {
            get
            {
                return this.designatedCIProductClassificationField;
            }
            set
            {
                this.designatedCIProductClassificationField = value;
            }
        }

        [XmlElement("IndividualCITradeProductInstance")]
        public CITradeProductInstanceType[] IndividualCITradeProductInstance
        {
            get
            {
                return this.individualCITradeProductInstanceField;
            }
            set
            {
                this.individualCITradeProductInstanceField = value;
            }
        }

        [XmlElement("IncludedCIReferencedProduct")]
        public CIReferencedProductType[] IncludedCIReferencedProduct
        {
            get
            {
                return this.includedCIReferencedProductField;
            }
            set
            {
                this.includedCIReferencedProductField = value;
            }
        }

        [XmlElement("OriginCITradeCountry")]
        public CITradeCountryType[] OriginCITradeCountry
        {
            get
            {
                return this.originCITradeCountryField;
            }
            set
            {
                this.originCITradeCountryField = value;
            }
        }

        [XmlElement("SuppliedFromCITradeCountry")]
        public CITradeCountryType[] SuppliedFromCITradeCountry
        {
            get
            {
                return this.suppliedFromCITradeCountryField;
            }
            set
            {
                this.suppliedFromCITradeCountryField = value;
            }
        }

        [XmlElement("FinalAssemblyCITradeCountry")]
        public CITradeCountryType[] FinalAssemblyCITradeCountry
        {
            get
            {
                return this.finalAssemblyCITradeCountryField;
            }
            set
            {
                this.finalAssemblyCITradeCountryField = value;
            }
        }

        [XmlElement("LinearCISpatialDimension")]
        public CISpatialDimensionType[] LinearCISpatialDimension
        {
            get
            {
                return this.linearCISpatialDimensionField;
            }
            set
            {
                this.linearCISpatialDimensionField = value;
            }
        }

        [XmlElement("MinimumLinearCISpatialDimension")]
        public CISpatialDimensionType[] MinimumLinearCISpatialDimension
        {
            get
            {
                return this.minimumLinearCISpatialDimensionField;
            }
            set
            {
                this.minimumLinearCISpatialDimensionField = value;
            }
        }

        [XmlElement("MaximumLinearCISpatialDimension")]
        public CISpatialDimensionType[] MaximumLinearCISpatialDimension
        {
            get
            {
                return this.maximumLinearCISpatialDimensionField;
            }
            set
            {
                this.maximumLinearCISpatialDimensionField = value;
            }
        }

        [XmlElement("ApplicableTradeProductSupplyChainPackaging")]
        public TradeProductSupplyChainPackagingType[] ApplicableTradeProductSupplyChainPackaging
        {
            get
            {
                return this.applicableTradeProductSupplyChainPackagingField;
            }
            set
            {
                this.applicableTradeProductSupplyChainPackagingField = value;
            }
        }

        [XmlElement("MarketingCampaignReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] MarketingCampaignReferenceCIReferencedDocument
        {
            get
            {
                return this.marketingCampaignReferenceCIReferencedDocumentField;
            }
            set
            {
                this.marketingCampaignReferenceCIReferencedDocumentField = value;
            }
        }

        [XmlElement("PresentationSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] PresentationSpecifiedBinaryFile
        {
            get
            {
                return this.presentationSpecifiedBinaryFileField;
            }
            set
            {
                this.presentationSpecifiedBinaryFileField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("Name", typeof(TextType), IsNullable = false)]
        public TextType[][] ApplicableKeyword
        {
            get
            {
                return this.applicableKeywordField;
            }
            set
            {
                this.applicableKeywordField = value;
            }
        }

        [XmlElement("AttachedProductSecurityTag")]
        public ProductSecurityTagType[] AttachedProductSecurityTag
        {
            get
            {
                return this.attachedProductSecurityTagField;
            }
            set
            {
                this.attachedProductSecurityTagField = value;
            }
        }

        [XmlElement("MarketingTradeProductFeature")]
        public TradeProductFeatureType[] MarketingTradeProductFeature
        {
            get
            {
                return this.marketingTradeProductFeatureField;
            }
            set
            {
                this.marketingTradeProductFeatureField = value;
            }
        }

        [XmlElement("MSDSReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] MSDSReferenceCIReferencedDocument
        {
            get
            {
                return this.mSDSReferenceCIReferencedDocumentField;
            }
            set
            {
                this.mSDSReferenceCIReferencedDocumentField = value;
            }
        }

        [XmlElement("AdditionalReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferenceCIReferencedDocument
        {
            get
            {
                return this.additionalReferenceCIReferencedDocumentField;
            }
            set
            {
                this.additionalReferenceCIReferencedDocumentField = value;
            }
        }

        [XmlElement("InformationCINote")]
        public CINoteType[] InformationCINote
        {
            get
            {
                return this.informationCINoteField;
            }
            set
            {
                this.informationCINoteField = value;
            }
        }

        [XmlElement("ApplicableTradeProductCertification")]
        public TradeProductCertificationType[] ApplicableTradeProductCertification
        {
            get
            {
                return this.applicableTradeProductCertificationField;
            }
            set
            {
                this.applicableTradeProductCertificationField = value;
            }
        }

        [XmlElement("ApplicableCIDisposalInstructions")]
        public CIDisposalInstructionsType[] ApplicableCIDisposalInstructions
        {
            get
            {
                return this.applicableCIDisposalInstructionsField;
            }
            set
            {
                this.applicableCIDisposalInstructionsField = value;
            }
        }
    }
}