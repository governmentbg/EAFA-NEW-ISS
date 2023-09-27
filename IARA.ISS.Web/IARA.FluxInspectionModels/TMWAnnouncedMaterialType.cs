namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWAnnouncedMaterial", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWAnnouncedMaterialType
    {

        private TextType nameField;

        private TextType compositionDescriptionField;

        private CodeType[] typeCodeField;

        private IndicatorType specialHandlingRequirementIndicatorField;

        private MassMeasurementType massMeasurementField;

        private VolumeMeasurementType volumeMeasurementField;

        private PhysicalCharacteristicType physicalApplicablePhysicalCharacteristicField;

        private AnnouncedPackageType specifiedAnnouncedPackageField;

        private TMWHandlingInstructionsType specialHandlingTMWHandlingInstructionsField;

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

        public AnnouncedPackageType SpecifiedAnnouncedPackage
        {
            get => this.specifiedAnnouncedPackageField;
            set => this.specifiedAnnouncedPackageField = value;
        }

        public TMWHandlingInstructionsType SpecialHandlingTMWHandlingInstructions
        {
            get => this.specialHandlingTMWHandlingInstructionsField;
            set => this.specialHandlingTMWHandlingInstructionsField = value;
        }
    }
}