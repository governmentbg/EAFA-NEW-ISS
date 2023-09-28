namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDLLogisticsPackage", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDLLogisticsPackageType
    {

        private IDType idField;

        private IDType globalIDField;

        private IDType parentIDField;

        private IDType seriesStartIDField;

        private IDType seriesEndIDField;

        private PackagingLevelCodeType levelCodeField;

        private PackageTypeCodeType typeCodeField;

        private TextType typeField;

        private QuantityType itemQuantityField;

        private MeasureType grossWeightMeasureField;

        private MeasureType nominalGrossWeightMeasureField;

        private MeasureType netWeightMeasureField;

        private MeasureType grossVolumeMeasureField;

        private MeasureType nominalGrossVolumeMeasureField;

        private NumericType sequenceNumericField;

        private TextType descriptionField;

        private TextType[] informationField;

        private CIDDLSupplyChainTradeLineItemType[] includedCIDDLSupplyChainTradeLineItemField;

        private CISupplyChainPackagingType[] usedCISupplyChainPackagingField;

        private LogisticsShippingMarksType[] physicalLogisticsShippingMarksField;

        private CISpatialDimensionType linearCISpatialDimensionField;

        private CIReferencedDocumentType[] despatchNoteAssociatedCIReferencedDocumentField;

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

        public IDType ParentID
        {
            get
            {
                return this.parentIDField;
            }
            set
            {
                this.parentIDField = value;
            }
        }

        public IDType SeriesStartID
        {
            get
            {
                return this.seriesStartIDField;
            }
            set
            {
                this.seriesStartIDField = value;
            }
        }

        public IDType SeriesEndID
        {
            get
            {
                return this.seriesEndIDField;
            }
            set
            {
                this.seriesEndIDField = value;
            }
        }

        public PackagingLevelCodeType LevelCode
        {
            get
            {
                return this.levelCodeField;
            }
            set
            {
                this.levelCodeField = value;
            }
        }

        public PackageTypeCodeType TypeCode
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

        public TextType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        public QuantityType ItemQuantity
        {
            get
            {
                return this.itemQuantityField;
            }
            set
            {
                this.itemQuantityField = value;
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

        public MeasureType NominalGrossWeightMeasure
        {
            get
            {
                return this.nominalGrossWeightMeasureField;
            }
            set
            {
                this.nominalGrossWeightMeasureField = value;
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

        public MeasureType GrossVolumeMeasure
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

        public MeasureType NominalGrossVolumeMeasure
        {
            get
            {
                return this.nominalGrossVolumeMeasureField;
            }
            set
            {
                this.nominalGrossVolumeMeasureField = value;
            }
        }

        public NumericType SequenceNumeric
        {
            get
            {
                return this.sequenceNumericField;
            }
            set
            {
                this.sequenceNumericField = value;
            }
        }

        public TextType Description
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

        [XmlElement("Information")]
        public TextType[] Information
        {
            get
            {
                return this.informationField;
            }
            set
            {
                this.informationField = value;
            }
        }

        [XmlElement("IncludedCIDDLSupplyChainTradeLineItem")]
        public CIDDLSupplyChainTradeLineItemType[] IncludedCIDDLSupplyChainTradeLineItem
        {
            get
            {
                return this.includedCIDDLSupplyChainTradeLineItemField;
            }
            set
            {
                this.includedCIDDLSupplyChainTradeLineItemField = value;
            }
        }

        [XmlElement("UsedCISupplyChainPackaging")]
        public CISupplyChainPackagingType[] UsedCISupplyChainPackaging
        {
            get
            {
                return this.usedCISupplyChainPackagingField;
            }
            set
            {
                this.usedCISupplyChainPackagingField = value;
            }
        }

        [XmlElement("PhysicalLogisticsShippingMarks")]
        public LogisticsShippingMarksType[] PhysicalLogisticsShippingMarks
        {
            get
            {
                return this.physicalLogisticsShippingMarksField;
            }
            set
            {
                this.physicalLogisticsShippingMarksField = value;
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

        [XmlElement("DespatchNoteAssociatedCIReferencedDocument")]
        public CIReferencedDocumentType[] DespatchNoteAssociatedCIReferencedDocument
        {
            get
            {
                return this.despatchNoteAssociatedCIReferencedDocumentField;
            }
            set
            {
                this.despatchNoteAssociatedCIReferencedDocumentField = value;
            }
        }
    }
}