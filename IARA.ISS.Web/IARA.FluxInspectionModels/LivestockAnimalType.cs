namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LivestockAnimal", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LivestockAnimalType
    {

        private IDType idField;

        private DateTimeType birthDateTimeField;

        private DateTimeType deathDateTimeField;

        private CodeType genderCodeField;

        private IDType motherIDField;

        private IDType registeredBirthLocationIDField;

        private CodeType speciesTypeCodeField;

        private SpecifiedClassificationType[] nationalAppearanceApplicableSpecifiedClassificationField;

        private SpecifiedClassificationType unifiedAppearanceApplicableSpecifiedClassificationField;

        private SpecifiedClassificationType[] nationalColourApplicableSpecifiedClassificationField;

        private SpecifiedClassificationType unifiedColourApplicableSpecifiedClassificationField;

        private AnimalIdentityType[] specifiedAnimalIdentityField;

        private VeterinarianEventType[] specifiedVeterinarianEventField;

        private AnimalHoldingEventType[] specifiedAnimalHoldingEventField;

        private CrossBorderAnimalMovementEventType[] specifiedCrossBorderAnimalMovementEventField;

        private AnimalGrantType[] awardedAnimalGrantField;

        private AnimalPassportDocumentType[] associatedAnimalPassportDocumentField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public DateTimeType BirthDateTime
        {
            get => this.birthDateTimeField;
            set => this.birthDateTimeField = value;
        }

        public DateTimeType DeathDateTime
        {
            get => this.deathDateTimeField;
            set => this.deathDateTimeField = value;
        }

        public CodeType GenderCode
        {
            get => this.genderCodeField;
            set => this.genderCodeField = value;
        }

        public IDType MotherID
        {
            get => this.motherIDField;
            set => this.motherIDField = value;
        }

        public IDType RegisteredBirthLocationID
        {
            get => this.registeredBirthLocationIDField;
            set => this.registeredBirthLocationIDField = value;
        }

        public CodeType SpeciesTypeCode
        {
            get => this.speciesTypeCodeField;
            set => this.speciesTypeCodeField = value;
        }

        [XmlElement("NationalAppearanceApplicableSpecifiedClassification")]
        public SpecifiedClassificationType[] NationalAppearanceApplicableSpecifiedClassification
        {
            get => this.nationalAppearanceApplicableSpecifiedClassificationField;
            set => this.nationalAppearanceApplicableSpecifiedClassificationField = value;
        }

        public SpecifiedClassificationType UnifiedAppearanceApplicableSpecifiedClassification
        {
            get => this.unifiedAppearanceApplicableSpecifiedClassificationField;
            set => this.unifiedAppearanceApplicableSpecifiedClassificationField = value;
        }

        [XmlElement("NationalColourApplicableSpecifiedClassification")]
        public SpecifiedClassificationType[] NationalColourApplicableSpecifiedClassification
        {
            get => this.nationalColourApplicableSpecifiedClassificationField;
            set => this.nationalColourApplicableSpecifiedClassificationField = value;
        }

        public SpecifiedClassificationType UnifiedColourApplicableSpecifiedClassification
        {
            get => this.unifiedColourApplicableSpecifiedClassificationField;
            set => this.unifiedColourApplicableSpecifiedClassificationField = value;
        }

        [XmlElement("SpecifiedAnimalIdentity")]
        public AnimalIdentityType[] SpecifiedAnimalIdentity
        {
            get => this.specifiedAnimalIdentityField;
            set => this.specifiedAnimalIdentityField = value;
        }

        [XmlElement("SpecifiedVeterinarianEvent")]
        public VeterinarianEventType[] SpecifiedVeterinarianEvent
        {
            get => this.specifiedVeterinarianEventField;
            set => this.specifiedVeterinarianEventField = value;
        }

        [XmlElement("SpecifiedAnimalHoldingEvent")]
        public AnimalHoldingEventType[] SpecifiedAnimalHoldingEvent
        {
            get => this.specifiedAnimalHoldingEventField;
            set => this.specifiedAnimalHoldingEventField = value;
        }

        [XmlElement("SpecifiedCrossBorderAnimalMovementEvent")]
        public CrossBorderAnimalMovementEventType[] SpecifiedCrossBorderAnimalMovementEvent
        {
            get => this.specifiedCrossBorderAnimalMovementEventField;
            set => this.specifiedCrossBorderAnimalMovementEventField = value;
        }

        [XmlElement("AwardedAnimalGrant")]
        public AnimalGrantType[] AwardedAnimalGrant
        {
            get => this.awardedAnimalGrantField;
            set => this.awardedAnimalGrantField = value;
        }

        [XmlElement("AssociatedAnimalPassportDocument")]
        public AnimalPassportDocumentType[] AssociatedAnimalPassportDocument
        {
            get => this.associatedAnimalPassportDocumentField;
            set => this.associatedAnimalPassportDocumentField = value;
        }
    }
}