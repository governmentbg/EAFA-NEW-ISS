namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWNotifiedMaterial", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWNotifiedMaterialType
    {

        private TextType nameField;

        private TextType compositionDescriptionField;

        private CodeType[] typeCodeField;

        private IndicatorType specialHandlingRequirementIndicatorField;

        private MeasureType estimatedMinimumMassMeasureField;

        private MeasureType estimatedMaximumMassMeasureField;

        private MeasureType estimatedMinimumVolumeMeasureField;

        private MeasureType estimatedMaximumVolumeMeasureField;

        private MassMeasurementType massMeasurementField;

        private VolumeMeasurementType volumeMeasurementField;

        private PhysicalCharacteristicType physicalApplicablePhysicalCharacteristicField;

        private TMWPackageType specifiedTMWPackageField;

        private TMWHandlingInstructionsType specialHandlingTMWHandlingInstructionsField;

        private RecoveryWasteMaterialType recoveryFractionRecoveryWasteMaterialField;

        private NonRecoveryWasteMaterialType nonRecoveryFractionNonRecoveryWasteMaterialField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType CompositionDescription
        {
            get => this.compositionDescriptionField;
            set => this.compositionDescriptionField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public IndicatorType SpecialHandlingRequirementIndicator
        {
            get => this.specialHandlingRequirementIndicatorField;
            set => this.specialHandlingRequirementIndicatorField = value;
        }

        public MeasureType EstimatedMinimumMassMeasure
        {
            get => this.estimatedMinimumMassMeasureField;
            set => this.estimatedMinimumMassMeasureField = value;
        }

        public MeasureType EstimatedMaximumMassMeasure
        {
            get => this.estimatedMaximumMassMeasureField;
            set => this.estimatedMaximumMassMeasureField = value;
        }

        public MeasureType EstimatedMinimumVolumeMeasure
        {
            get => this.estimatedMinimumVolumeMeasureField;
            set => this.estimatedMinimumVolumeMeasureField = value;
        }

        public MeasureType EstimatedMaximumVolumeMeasure
        {
            get => this.estimatedMaximumVolumeMeasureField;
            set => this.estimatedMaximumVolumeMeasureField = value;
        }

        public MassMeasurementType MassMeasurement
        {
            get => this.massMeasurementField;
            set => this.massMeasurementField = value;
        }

        public VolumeMeasurementType VolumeMeasurement
        {
            get => this.volumeMeasurementField;
            set => this.volumeMeasurementField = value;
        }

        public PhysicalCharacteristicType PhysicalApplicablePhysicalCharacteristic
        {
            get => this.physicalApplicablePhysicalCharacteristicField;
            set => this.physicalApplicablePhysicalCharacteristicField = value;
        }

        public TMWPackageType SpecifiedTMWPackage
        {
            get => this.specifiedTMWPackageField;
            set => this.specifiedTMWPackageField = value;
        }

        public TMWHandlingInstructionsType SpecialHandlingTMWHandlingInstructions
        {
            get => this.specialHandlingTMWHandlingInstructionsField;
            set => this.specialHandlingTMWHandlingInstructionsField = value;
        }

        public RecoveryWasteMaterialType RecoveryFractionRecoveryWasteMaterial
        {
            get => this.recoveryFractionRecoveryWasteMaterialField;
            set => this.recoveryFractionRecoveryWasteMaterialField = value;
        }

        public NonRecoveryWasteMaterialType NonRecoveryFractionNonRecoveryWasteMaterial
        {
            get => this.nonRecoveryFractionNonRecoveryWasteMaterialField;
            set => this.nonRecoveryFractionNonRecoveryWasteMaterialField = value;
        }
    }
}