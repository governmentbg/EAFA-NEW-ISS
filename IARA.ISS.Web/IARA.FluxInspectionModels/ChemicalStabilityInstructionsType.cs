namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ChemicalStabilityInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ChemicalStabilityInstructionsType
    {

        private IndicatorType riskMitigationStorageRequirementIndicatorField;

        private IndicatorType hazardousPolymerizationRiskMitigationStorageRequirementIndicatorField;

        private TextType descriptionField;

        private TextType conditionToAvoidDescriptionField;

        private TextType hazardousDecompositionProductItemNameField;

        private TextType hazardousPolymerizationItemNameField;

        public IndicatorType RiskMitigationStorageRequirementIndicator
        {
            get
            {
                return this.riskMitigationStorageRequirementIndicatorField;
            }
            set
            {
                this.riskMitigationStorageRequirementIndicatorField = value;
            }
        }

        public IndicatorType HazardousPolymerizationRiskMitigationStorageRequirementIndicator
        {
            get
            {
                return this.hazardousPolymerizationRiskMitigationStorageRequirementIndicatorField;
            }
            set
            {
                this.hazardousPolymerizationRiskMitigationStorageRequirementIndicatorField = value;
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

        public TextType ConditionToAvoidDescription
        {
            get
            {
                return this.conditionToAvoidDescriptionField;
            }
            set
            {
                this.conditionToAvoidDescriptionField = value;
            }
        }

        public TextType HazardousDecompositionProductItemName
        {
            get
            {
                return this.hazardousDecompositionProductItemNameField;
            }
            set
            {
                this.hazardousDecompositionProductItemNameField = value;
            }
        }

        public TextType HazardousPolymerizationItemName
        {
            get
            {
                return this.hazardousPolymerizationItemNameField;
            }
            set
            {
                this.hazardousPolymerizationItemNameField = value;
            }
        }
    }
}