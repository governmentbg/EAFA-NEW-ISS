namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MSDSRegulatedGoods", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MSDSRegulatedGoodsType
    {

        private TextType disclaimerField;

        private TextType labelField;

        private TextType nameField;

        private TextType additionalNameField;

        private IDType fCSIDField;

        private IDType additionalIDField;

        private IDType nIINIDField;

        private IDType lIINIDField;

        private IDType activityControlIDField;

        private IDType federalSupplyScheduleSpecialItemIDField;

        private TextType informationSourceField;

        private TextType useDescriptionField;

        private TextType descriptionField;

        private IDType originCountryIDField;

        private IDType cAGEManufacturerIDField;

        private IDType cAGEContractorIDField;

        private IDType contractIDField;

        private IDType solicitationIDField;

        private IDType specificationDocumentIDField;

        private HazardRatingType environmentalHazardRatingField;

        private DistinctChemicalType[] componentDistinctChemicalField;

        private ToxicologicalHazardousMaterialType applicableToxicologicalHazardousMaterialField;

        private MSDSHazardousMaterialType[] applicableMSDSHazardousMaterialField;

        private ProductInformationContactType designatedProductInformationContactField;

        private FirstAidInstructionsType providedFirstAidInstructionsField;

        private FireFightingInstructionsType providedFireFightingInstructionsField;

        private AccidentalReleaseMeasureInstructionsType providedAccidentalReleaseMeasureInstructionsField;

        private HazardousMaterialHandlingInstructionsType providedHazardousMaterialHandlingInstructionsField;

        private HazardousMaterialStorageInstructionsType providedHazardousMaterialStorageInstructionsField;

        private ExposureControlInstructionsType providedExposureControlInstructionsField;

        private PersonalProtectionInstructionsType providedPersonalProtectionInstructionsField;

        private ChemicalStabilityInstructionsType providedChemicalStabilityInstructionsField;

        private ChemicalReactivityInstructionsType providedChemicalReactivityInstructionsField;

        private DisposalInstructionsType providedDisposalInstructionsField;

        private HazardousGoodsCharacteristicType applicableHazardousGoodsCharacteristicField;

        private EcologicalImpactGoodsCharacteristicType applicableEcologicalImpactGoodsCharacteristicField;

        private SolubilityGoodsCharacteristicType applicableSolubilityGoodsCharacteristicField;

        private VolatileOrganicCompoundGoodsCharacteristicType[] applicableVolatileOrganicCompoundGoodsCharacteristicField;

        private RegulatedDangerousGoodsType applicableRegulatedDangerousGoodsField;

        private SpecificationDocumentType[] providedSpecificationDocumentField;

        private ReferencedRegulationType applicableReferencedRegulationField;

        public TextType Disclaimer
        {
            get => this.disclaimerField;
            set => this.disclaimerField = value;
        }

        public TextType Label
        {
            get => this.labelField;
            set => this.labelField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType AdditionalName
        {
            get => this.additionalNameField;
            set => this.additionalNameField = value;
        }

        public IDType FCSID
        {
            get => this.fCSIDField;
            set => this.fCSIDField = value;
        }

        public IDType AdditionalID
        {
            get => this.additionalIDField;
            set => this.additionalIDField = value;
        }

        public IDType NIINID
        {
            get => this.nIINIDField;
            set => this.nIINIDField = value;
        }

        public IDType LIINID
        {
            get => this.lIINIDField;
            set => this.lIINIDField = value;
        }

        public IDType ActivityControlID
        {
            get => this.activityControlIDField;
            set => this.activityControlIDField = value;
        }

        public IDType FederalSupplyScheduleSpecialItemID
        {
            get => this.federalSupplyScheduleSpecialItemIDField;
            set => this.federalSupplyScheduleSpecialItemIDField = value;
        }

        public TextType InformationSource
        {
            get => this.informationSourceField;
            set => this.informationSourceField = value;
        }

        public TextType UseDescription
        {
            get => this.useDescriptionField;
            set => this.useDescriptionField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public IDType OriginCountryID
        {
            get => this.originCountryIDField;
            set => this.originCountryIDField = value;
        }

        public IDType CAGEManufacturerID
        {
            get => this.cAGEManufacturerIDField;
            set => this.cAGEManufacturerIDField = value;
        }

        public IDType CAGEContractorID
        {
            get => this.cAGEContractorIDField;
            set => this.cAGEContractorIDField = value;
        }

        public IDType ContractID
        {
            get => this.contractIDField;
            set => this.contractIDField = value;
        }

        public IDType SolicitationID
        {
            get => this.solicitationIDField;
            set => this.solicitationIDField = value;
        }

        public IDType SpecificationDocumentID
        {
            get => this.specificationDocumentIDField;
            set => this.specificationDocumentIDField = value;
        }

        public HazardRatingType EnvironmentalHazardRating
        {
            get => this.environmentalHazardRatingField;
            set => this.environmentalHazardRatingField = value;
        }

        [XmlElement("ComponentDistinctChemical")]
        public DistinctChemicalType[] ComponentDistinctChemical
        {
            get => this.componentDistinctChemicalField;
            set => this.componentDistinctChemicalField = value;
        }

        public ToxicologicalHazardousMaterialType ApplicableToxicologicalHazardousMaterial
        {
            get => this.applicableToxicologicalHazardousMaterialField;
            set => this.applicableToxicologicalHazardousMaterialField = value;
        }

        [XmlElement("ApplicableMSDSHazardousMaterial")]
        public MSDSHazardousMaterialType[] ApplicableMSDSHazardousMaterial
        {
            get => this.applicableMSDSHazardousMaterialField;
            set => this.applicableMSDSHazardousMaterialField = value;
        }

        public ProductInformationContactType DesignatedProductInformationContact
        {
            get => this.designatedProductInformationContactField;
            set => this.designatedProductInformationContactField = value;
        }

        public FirstAidInstructionsType ProvidedFirstAidInstructions
        {
            get => this.providedFirstAidInstructionsField;
            set => this.providedFirstAidInstructionsField = value;
        }

        public FireFightingInstructionsType ProvidedFireFightingInstructions
        {
            get => this.providedFireFightingInstructionsField;
            set => this.providedFireFightingInstructionsField = value;
        }

        public AccidentalReleaseMeasureInstructionsType ProvidedAccidentalReleaseMeasureInstructions
        {
            get => this.providedAccidentalReleaseMeasureInstructionsField;
            set => this.providedAccidentalReleaseMeasureInstructionsField = value;
        }

        public HazardousMaterialHandlingInstructionsType ProvidedHazardousMaterialHandlingInstructions
        {
            get => this.providedHazardousMaterialHandlingInstructionsField;
            set => this.providedHazardousMaterialHandlingInstructionsField = value;
        }

        public HazardousMaterialStorageInstructionsType ProvidedHazardousMaterialStorageInstructions
        {
            get => this.providedHazardousMaterialStorageInstructionsField;
            set => this.providedHazardousMaterialStorageInstructionsField = value;
        }

        public ExposureControlInstructionsType ProvidedExposureControlInstructions
        {
            get => this.providedExposureControlInstructionsField;
            set => this.providedExposureControlInstructionsField = value;
        }

        public PersonalProtectionInstructionsType ProvidedPersonalProtectionInstructions
        {
            get => this.providedPersonalProtectionInstructionsField;
            set => this.providedPersonalProtectionInstructionsField = value;
        }

        public ChemicalStabilityInstructionsType ProvidedChemicalStabilityInstructions
        {
            get => this.providedChemicalStabilityInstructionsField;
            set => this.providedChemicalStabilityInstructionsField = value;
        }

        public ChemicalReactivityInstructionsType ProvidedChemicalReactivityInstructions
        {
            get => this.providedChemicalReactivityInstructionsField;
            set => this.providedChemicalReactivityInstructionsField = value;
        }

        public DisposalInstructionsType ProvidedDisposalInstructions
        {
            get => this.providedDisposalInstructionsField;
            set => this.providedDisposalInstructionsField = value;
        }

        public HazardousGoodsCharacteristicType ApplicableHazardousGoodsCharacteristic
        {
            get => this.applicableHazardousGoodsCharacteristicField;
            set => this.applicableHazardousGoodsCharacteristicField = value;
        }

        public EcologicalImpactGoodsCharacteristicType ApplicableEcologicalImpactGoodsCharacteristic
        {
            get => this.applicableEcologicalImpactGoodsCharacteristicField;
            set => this.applicableEcologicalImpactGoodsCharacteristicField = value;
        }

        public SolubilityGoodsCharacteristicType ApplicableSolubilityGoodsCharacteristic
        {
            get => this.applicableSolubilityGoodsCharacteristicField;
            set => this.applicableSolubilityGoodsCharacteristicField = value;
        }

        [XmlElement("ApplicableVolatileOrganicCompoundGoodsCharacteristic")]
        public VolatileOrganicCompoundGoodsCharacteristicType[] ApplicableVolatileOrganicCompoundGoodsCharacteristic
        {
            get => this.applicableVolatileOrganicCompoundGoodsCharacteristicField;
            set => this.applicableVolatileOrganicCompoundGoodsCharacteristicField = value;
        }

        public RegulatedDangerousGoodsType ApplicableRegulatedDangerousGoods
        {
            get => this.applicableRegulatedDangerousGoodsField;
            set => this.applicableRegulatedDangerousGoodsField = value;
        }

        [XmlElement("ProvidedSpecificationDocument")]
        public SpecificationDocumentType[] ProvidedSpecificationDocument
        {
            get => this.providedSpecificationDocumentField;
            set => this.providedSpecificationDocumentField = value;
        }

        public ReferencedRegulationType ApplicableReferencedRegulation
        {
            get => this.applicableReferencedRegulationField;
            set => this.applicableReferencedRegulationField = value;
        }
    }
}