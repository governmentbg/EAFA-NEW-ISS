namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("HazardousGoodsCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class HazardousGoodsCharacteristicType
    {

        private TextType appearanceDescriptionField;

        private TextType odourDescriptionField;

        private TextType colourDescriptionField;

        private TextType physicalStateDescriptionField;

        private MeasureType corrosionRateFeatureMeasureField;

        private MeasureType saturatedVapourConcentrationFeatureMeasureField;

        private MeasureType octanolWaterCoefficientFeatureMeasureField;

        private MeasureType particleSizeFeatureMeasureField;

        private MeasureType heatFeatureMeasureField;

        private MeasureType bulkDensityFeatureMeasureField;

        private BoilingPointRangeMeasurementType specifiedBoilingPointRangeMeasurementField;

        private VapourPressureRangeMeasurementType specifiedVapourPressureRangeMeasurementField;

        private VapourDensityRangeMeasurementType specifiedVapourDensityRangeMeasurementField;

        private DecompositionTemperatureRangeMeasurementType specifiedDecompositionTemperatureRangeMeasurementField;

        private DensityRangeMeasurementType specifiedDensityRangeMeasurementField;

        private SpecificGravityRangeMeasurementType specifiedSpecificGravityRangeMeasurementField;

        private FreezingPointRangeMeasurementType specifiedFreezingPointRangeMeasurementField;

        private PHRangeMeasurementType specifiedPHRangeMeasurementField;

        private ViscosityRangeMeasurementType specifiedViscosityRangeMeasurementField;

        private EvaporationRangeMeasurementType specifiedEvaporationRangeMeasurementField;

        private PartitionCoefficientRangeMeasurementType specifiedPartitionCoefficientRangeMeasurementField;

        private UnspecifiedPhysicalPropertyRangeMeasurementType specifiedUnspecifiedPhysicalPropertyRangeMeasurementField;

        private UnspecifiedChemicalPropertyRangeMeasurementType specifiedUnspecifiedChemicalPropertyRangeMeasurementField;

        private FlashpointRangeMeasurementType[] specifiedFlashpointRangeMeasurementField;

        private AutoignitionRangeMeasurementType[] specifiedAutoignitionRangeMeasurementField;

        private ExplosiveLimitsConcentrationRangeMeasurementType[] specifiedExplosiveLimitsConcentrationRangeMeasurementField;

        public TextType AppearanceDescription
        {
            get => this.appearanceDescriptionField;
            set => this.appearanceDescriptionField = value;
        }

        public TextType OdourDescription
        {
            get => this.odourDescriptionField;
            set => this.odourDescriptionField = value;
        }

        public TextType ColourDescription
        {
            get => this.colourDescriptionField;
            set => this.colourDescriptionField = value;
        }

        public TextType PhysicalStateDescription
        {
            get => this.physicalStateDescriptionField;
            set => this.physicalStateDescriptionField = value;
        }

        public MeasureType CorrosionRateFeatureMeasure
        {
            get => this.corrosionRateFeatureMeasureField;
            set => this.corrosionRateFeatureMeasureField = value;
        }

        public MeasureType SaturatedVapourConcentrationFeatureMeasure
        {
            get => this.saturatedVapourConcentrationFeatureMeasureField;
            set => this.saturatedVapourConcentrationFeatureMeasureField = value;
        }

        public MeasureType OctanolWaterCoefficientFeatureMeasure
        {
            get => this.octanolWaterCoefficientFeatureMeasureField;
            set => this.octanolWaterCoefficientFeatureMeasureField = value;
        }

        public MeasureType ParticleSizeFeatureMeasure
        {
            get => this.particleSizeFeatureMeasureField;
            set => this.particleSizeFeatureMeasureField = value;
        }

        public MeasureType HeatFeatureMeasure
        {
            get => this.heatFeatureMeasureField;
            set => this.heatFeatureMeasureField = value;
        }

        public MeasureType BulkDensityFeatureMeasure
        {
            get => this.bulkDensityFeatureMeasureField;
            set => this.bulkDensityFeatureMeasureField = value;
        }

        public BoilingPointRangeMeasurementType SpecifiedBoilingPointRangeMeasurement
        {
            get => this.specifiedBoilingPointRangeMeasurementField;
            set => this.specifiedBoilingPointRangeMeasurementField = value;
        }

        public VapourPressureRangeMeasurementType SpecifiedVapourPressureRangeMeasurement
        {
            get => this.specifiedVapourPressureRangeMeasurementField;
            set => this.specifiedVapourPressureRangeMeasurementField = value;
        }

        public VapourDensityRangeMeasurementType SpecifiedVapourDensityRangeMeasurement
        {
            get => this.specifiedVapourDensityRangeMeasurementField;
            set => this.specifiedVapourDensityRangeMeasurementField = value;
        }

        public DecompositionTemperatureRangeMeasurementType SpecifiedDecompositionTemperatureRangeMeasurement
        {
            get => this.specifiedDecompositionTemperatureRangeMeasurementField;
            set => this.specifiedDecompositionTemperatureRangeMeasurementField = value;
        }

        public DensityRangeMeasurementType SpecifiedDensityRangeMeasurement
        {
            get => this.specifiedDensityRangeMeasurementField;
            set => this.specifiedDensityRangeMeasurementField = value;
        }

        public SpecificGravityRangeMeasurementType SpecifiedSpecificGravityRangeMeasurement
        {
            get => this.specifiedSpecificGravityRangeMeasurementField;
            set => this.specifiedSpecificGravityRangeMeasurementField = value;
        }

        public FreezingPointRangeMeasurementType SpecifiedFreezingPointRangeMeasurement
        {
            get => this.specifiedFreezingPointRangeMeasurementField;
            set => this.specifiedFreezingPointRangeMeasurementField = value;
        }

        public PHRangeMeasurementType SpecifiedPHRangeMeasurement
        {
            get => this.specifiedPHRangeMeasurementField;
            set => this.specifiedPHRangeMeasurementField = value;
        }

        public ViscosityRangeMeasurementType SpecifiedViscosityRangeMeasurement
        {
            get => this.specifiedViscosityRangeMeasurementField;
            set => this.specifiedViscosityRangeMeasurementField = value;
        }

        public EvaporationRangeMeasurementType SpecifiedEvaporationRangeMeasurement
        {
            get => this.specifiedEvaporationRangeMeasurementField;
            set => this.specifiedEvaporationRangeMeasurementField = value;
        }

        public PartitionCoefficientRangeMeasurementType SpecifiedPartitionCoefficientRangeMeasurement
        {
            get => this.specifiedPartitionCoefficientRangeMeasurementField;
            set => this.specifiedPartitionCoefficientRangeMeasurementField = value;
        }

        public UnspecifiedPhysicalPropertyRangeMeasurementType SpecifiedUnspecifiedPhysicalPropertyRangeMeasurement
        {
            get => this.specifiedUnspecifiedPhysicalPropertyRangeMeasurementField;
            set => this.specifiedUnspecifiedPhysicalPropertyRangeMeasurementField = value;
        }

        public UnspecifiedChemicalPropertyRangeMeasurementType SpecifiedUnspecifiedChemicalPropertyRangeMeasurement
        {
            get => this.specifiedUnspecifiedChemicalPropertyRangeMeasurementField;
            set => this.specifiedUnspecifiedChemicalPropertyRangeMeasurementField = value;
        }

        [XmlElement("SpecifiedFlashpointRangeMeasurement")]
        public FlashpointRangeMeasurementType[] SpecifiedFlashpointRangeMeasurement
        {
            get => this.specifiedFlashpointRangeMeasurementField;
            set => this.specifiedFlashpointRangeMeasurementField = value;
        }

        [XmlElement("SpecifiedAutoignitionRangeMeasurement")]
        public AutoignitionRangeMeasurementType[] SpecifiedAutoignitionRangeMeasurement
        {
            get => this.specifiedAutoignitionRangeMeasurementField;
            set => this.specifiedAutoignitionRangeMeasurementField = value;
        }

        [XmlElement("SpecifiedExplosiveLimitsConcentrationRangeMeasurement")]
        public ExplosiveLimitsConcentrationRangeMeasurementType[] SpecifiedExplosiveLimitsConcentrationRangeMeasurement
        {
            get => this.specifiedExplosiveLimitsConcentrationRangeMeasurementField;
            set => this.specifiedExplosiveLimitsConcentrationRangeMeasurementField = value;
        }
    }
}