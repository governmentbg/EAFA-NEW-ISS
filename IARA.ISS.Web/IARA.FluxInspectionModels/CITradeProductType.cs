namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeProductType
    {

        private IDType idField;

        private IDType globalIDField;

        private IDType sellerAssignedIDField;

        private IDType buyerAssignedIDField;

        private IDType manufacturerAssignedIDField;

        private TextType[] nameField;

        private TextType tradeNameField;

        private TextType[] descriptionField;

        private CodeType typeCodeField;

        private MeasureType netWeightMeasureField;

        private MeasureType grossWeightMeasureField;

        private MeasureType drainedNetWeightMeasureField;

        private TextType brandNameField;

        private TextType subBrandNameField;

        private IDType[] productGroupIDField;

        private MeasureType areaDensityMeasureField;

        private CodeType colourCodeField;

        private TextType[] colourDescriptionField;

        private TextType[] useDescriptionField;

        private TextType[] designationField;

        private TextType[] endItemNameField;

        private string latestProductDataChangeDateTimeField;

        private CodeType[] endItemTypeCodeField;

        private IndicatorType variableMeasureIndicatorField;

        private TextType[] additionalDescriptionField;

        private IDType[] batchIDField;

        private TextType[] commonNameField;

        private TextType[] conciseDescriptionField;

        private IndicatorType configurableIndicatorField;

        private QuantityType contentUnitQuantityField;

        private IDType[] customerAssignedIDField;

        private MeasureType[] grossVolumeMeasureField;

        private IDType[] industryAssignedIDField;

        private IDType[] modelIDField;

        private MeasureType[] netVolumeMeasureField;

        private CodeType recyclingTypeCodeField;

        private CodeType[] statusCodeField;

        private CodeType[] unitTypeCodeField;

        private IndicatorType exportIndicatorField;

        private CIProductCharacteristicType[] applicableCIProductCharacteristicField;

        private CIMaterialGoodsCharacteristicType[] applicableCIMaterialGoodsCharacteristicField;

        private CIProductClassificationType[] designatedCIProductClassificationField;

        private CITradeProductInstanceType[] individualCITradeProductInstanceField;

        private CIReferencedDocumentType[] certificationEvidenceReferenceCIReferencedDocumentField;

        private CIReferencedDocumentType[] inspectionReferenceCIReferencedDocumentField;

        private CITradeCountryType originCITradeCountryField;

        private CISpatialDimensionType linearCISpatialDimensionField;

        private CISpatialDimensionType minimumLinearCISpatialDimensionField;

        private CISpatialDimensionType maximumLinearCISpatialDimensionField;

        private CITradePartyType manufacturerCITradePartyField;

        private CIReferencedDocumentType mSDSReferenceCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferenceCIReferencedDocumentField;

        private CINoteType[] informationCINoteField;

        private CITradePartyType brandOwnerCITradePartyField;

        private CITradePartyType legalRightsOwnerCITradePartyField;

        private SpecifiedBinaryFileType[] presentationSpecifiedBinaryFileField;

        private CIReferencedDocumentType[] buyerSuppliedPartsReferenceCIReferencedDocumentField;

        public IDType ID
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

        public IDType GlobalID
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

        public IDType ManufacturerAssignedID
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

        public TextType TradeName
        {
            get
            {
                return this.tradeNameField;
            }
            set
            {
                this.tradeNameField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public MeasureType NetWeightMeasure
        {
            get
            {
                return this.netWeightMeasureField;
            }
            set
            {
                this.netWeightMeasureField = value;
            }
        }

        public MeasureType GrossWeightMeasure
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

        [XmlElement("ProductGroupID")]
        public IDType[] ProductGroupID
        {
            get
            {
                return this.productGroupIDField;
            }
            set
            {
                this.productGroupIDField = value;
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

        [XmlElement("UseDescription")]
        public TextType[] UseDescription
        {
            get
            {
                return this.useDescriptionField;
            }
            set
            {
                this.useDescriptionField = value;
            }
        }

        [XmlElement("Designation")]
        public TextType[] Designation
        {
            get
            {
                return this.designationField;
            }
            set
            {
                this.designationField = value;
            }
        }

        [XmlElement("EndItemName")]
        public TextType[] EndItemName
        {
            get
            {
                return this.endItemNameField;
            }
            set
            {
                this.endItemNameField = value;
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

        [XmlElement("EndItemTypeCode")]
        public CodeType[] EndItemTypeCode
        {
            get
            {
                return this.endItemTypeCodeField;
            }
            set
            {
                this.endItemTypeCodeField = value;
            }
        }

        public IndicatorType VariableMeasureIndicator
        {
            get
            {
                return this.variableMeasureIndicatorField;
            }
            set
            {
                this.variableMeasureIndicatorField = value;
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

        [XmlElement("CommonName")]
        public TextType[] CommonName
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

        [XmlElement("CustomerAssignedID")]
        public IDType[] CustomerAssignedID
        {
            get
            {
                return this.customerAssignedIDField;
            }
            set
            {
                this.customerAssignedIDField = value;
            }
        }

        [XmlElement("GrossVolumeMeasure")]
        public MeasureType[] GrossVolumeMeasure
        {
            get
            {
                return this.grossVolumeMeasureField;
            }
            set
            {
                this.grossVolumeMeasureField = value;
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

        [XmlElement("ModelID")]
        public IDType[] ModelID
        {
            get
            {
                return this.modelIDField;
            }
            set
            {
                this.modelIDField = value;
            }
        }

        [XmlElement("NetVolumeMeasure")]
        public MeasureType[] NetVolumeMeasure
        {
            get
            {
                return this.netVolumeMeasureField;
            }
            set
            {
                this.netVolumeMeasureField = value;
            }
        }

        public CodeType RecyclingTypeCode
        {
            get
            {
                return this.recyclingTypeCodeField;
            }
            set
            {
                this.recyclingTypeCodeField = value;
            }
        }

        [XmlElement("StatusCode")]
        public CodeType[] StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
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

        public IndicatorType ExportIndicator
        {
            get
            {
                return this.exportIndicatorField;
            }
            set
            {
                this.exportIndicatorField = value;
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

        [XmlElement("CertificationEvidenceReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] CertificationEvidenceReferenceCIReferencedDocument
        {
            get
            {
                return this.certificationEvidenceReferenceCIReferencedDocumentField;
            }
            set
            {
                this.certificationEvidenceReferenceCIReferencedDocumentField = value;
            }
        }

        [XmlElement("InspectionReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] InspectionReferenceCIReferencedDocument
        {
            get
            {
                return this.inspectionReferenceCIReferencedDocumentField;
            }
            set
            {
                this.inspectionReferenceCIReferencedDocumentField = value;
            }
        }

        public CITradeCountryType OriginCITradeCountry
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

        public CISpatialDimensionType LinearCISpatialDimension
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

        public CISpatialDimensionType MinimumLinearCISpatialDimension
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

        public CISpatialDimensionType MaximumLinearCISpatialDimension
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

        public CITradePartyType ManufacturerCITradeParty
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

        public CIReferencedDocumentType MSDSReferenceCIReferencedDocument
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

        [XmlElement("BuyerSuppliedPartsReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] BuyerSuppliedPartsReferenceCIReferencedDocument
        {
            get
            {
                return this.buyerSuppliedPartsReferenceCIReferencedDocumentField;
            }
            set
            {
                this.buyerSuppliedPartsReferenceCIReferencedDocumentField = value;
            }
        }
    }
}