namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISLogisticsPackage", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISLogisticsPackageType
    {

        private QuantityType itemQuantityField;

        private MeasureType grossWeightMeasureField;

        private MeasureType nominalGrossWeightMeasureField;

        private MeasureType netWeightMeasureField;

        private MeasureType grossVolumeMeasureField;

        private MeasureType nominalGrossVolumeMeasureField;

        private PackagingLevelCodeType levelCodeField;

        private PackageTypeCodeType typeCodeField;

        private TextType typeField;

        private NumericType sequenceNumericField;

        private IDType idField;

        private IDType parentIDField;

        private IDType globalIDField;

        private TextType descriptionField;

        private TextType[] informationField;

        private IDType seriesStartIDField;

        private IDType seriesEndIDField;

        private QuantityType[] perPackageUnitQuantityField;

        public QuantityType ItemQuantity
        {
            get => this.itemQuantityField;
            set => this.itemQuantityField = value;
        }

        public MeasureType GrossWeightMeasure
        {
            get => this.grossWeightMeasureField;
            set => this.grossWeightMeasureField = value;
        }

        public MeasureType NominalGrossWeightMeasure
        {
            get => this.nominalGrossWeightMeasureField;
            set => this.nominalGrossWeightMeasureField = value;
        }

        public MeasureType NetWeightMeasure
        {
            get => this.netWeightMeasureField;
            set => this.netWeightMeasureField = value;
        }

        public MeasureType GrossVolumeMeasure
        {
            get => this.grossVolumeMeasureField;
            set => this.grossVolumeMeasureField = value;
        }

        public MeasureType NominalGrossVolumeMeasure
        {
            get => this.nominalGrossVolumeMeasureField;
            set => this.nominalGrossVolumeMeasureField = value;
        }

        public PackagingLevelCodeType LevelCode
        {
            get => this.levelCodeField;
            set => this.levelCodeField = value;
        }

        public PackageTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType Type
        {
            get => this.typeField;
            set => this.typeField = value;
        }

        public NumericType SequenceNumeric
        {
            get => this.sequenceNumericField;
            set => this.sequenceNumericField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType ParentID
        {
            get => this.parentIDField;
            set => this.parentIDField = value;
        }

        public IDType GlobalID
        {
            get => this.globalIDField;
            set => this.globalIDField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("Information")]
        public TextType[] Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public IDType SeriesStartID
        {
            get => this.seriesStartIDField;
            set => this.seriesStartIDField = value;
        }

        public IDType SeriesEndID
        {
            get => this.seriesEndIDField;
            set => this.seriesEndIDField = value;
        }

        [XmlElement("PerPackageUnitQuantity")]
        public QuantityType[] PerPackageUnitQuantity
        {
            get => this.perPackageUnitQuantityField;
            set => this.perPackageUnitQuantityField = value;
        }
    }
}