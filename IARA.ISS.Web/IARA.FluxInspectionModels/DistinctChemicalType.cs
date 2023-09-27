namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DistinctChemical", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DistinctChemicalType
    {

        private TextType scientificNameField;

        private TextType[] commonNameField;

        private TextType[] synonymNameField;

        private TextType familyNameField;

        private MeasureType molecularWeightMeasureField;

        private CodeType typeCodeField;

        private TextType formulaDescriptionField;

        private IngredientRangeMeasurementType[] presenceIngredientRangeMeasurementField;

        private ToxicologicalHazardousMaterialType[] applicableToxicologicalHazardousMaterialField;

        public TextType ScientificName
        {
            get => this.scientificNameField;
            set => this.scientificNameField = value;
        }

        [XmlElement("CommonName")]
        public TextType[] CommonName
        {
            get => this.commonNameField;
            set => this.commonNameField = value;
        }

        [XmlElement("SynonymName")]
        public TextType[] SynonymName
        {
            get => this.synonymNameField;
            set => this.synonymNameField = value;
        }

        public TextType FamilyName
        {
            get => this.familyNameField;
            set => this.familyNameField = value;
        }

        public MeasureType MolecularWeightMeasure
        {
            get => this.molecularWeightMeasureField;
            set => this.molecularWeightMeasureField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType FormulaDescription
        {
            get => this.formulaDescriptionField;
            set => this.formulaDescriptionField = value;
        }

        [XmlElement("PresenceIngredientRangeMeasurement")]
        public IngredientRangeMeasurementType[] PresenceIngredientRangeMeasurement
        {
            get => this.presenceIngredientRangeMeasurementField;
            set => this.presenceIngredientRangeMeasurementField = value;
        }

        [XmlElement("ApplicableToxicologicalHazardousMaterial")]
        public ToxicologicalHazardousMaterialType[] ApplicableToxicologicalHazardousMaterial
        {
            get => this.applicableToxicologicalHazardousMaterialField;
            set => this.applicableToxicologicalHazardousMaterialField = value;
        }
    }
}