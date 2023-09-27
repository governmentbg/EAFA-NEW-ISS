namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReferencedLogisticsPackage", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReferencedLogisticsPackageType
    {

        private QuantityType itemQuantityField;

        private PackagingLevelCodeType levelCodeField;

        private PackageTypeCodeType typeCodeField;

        private NumericType sequenceNumericField;

        private IDType idField;

        private IDType globalIDField;

        private IDType parentIDField;

        private IDType seriesStartIDField;

        private IDType seriesEndIDField;

        private CodeType colourCodeField;

        private TextType[] typeField;

        private WeightUnitMeasureType grossWeightMeasureField;

        private WeightUnitMeasureType nominalGrossWeightMeasureField;

        private WeightUnitMeasureType netWeightMeasureField;

        private VolumeUnitMeasureType grossVolumeMeasureField;

        private VolumeUnitMeasureType nominalGrossVolumeMeasureField;

        private TextType[] descriptionField;

        private TextType[] informationField;

        private QuantityType[] perPackageUnitQuantityField;

        private LogisticsShippingMarksType[] physicalLogisticsShippingMarksField;

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

        [XmlElement("Type")]
        public TextType[] Type
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

        public WeightUnitMeasureType GrossWeightMeasure
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

        public WeightUnitMeasureType NominalGrossWeightMeasure
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

        public WeightUnitMeasureType NetWeightMeasure
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

        public VolumeUnitMeasureType GrossVolumeMeasure
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

        public VolumeUnitMeasureType NominalGrossVolumeMeasure
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

        [XmlElement("PerPackageUnitQuantity")]
        public QuantityType[] PerPackageUnitQuantity
        {
            get
            {
                return this.perPackageUnitQuantityField;
            }
            set
            {
                this.perPackageUnitQuantityField = value;
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
    }
}