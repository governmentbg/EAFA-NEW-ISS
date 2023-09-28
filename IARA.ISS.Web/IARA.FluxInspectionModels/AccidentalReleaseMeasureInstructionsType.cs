namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AccidentalReleaseMeasureInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AccidentalReleaseMeasureInstructionsType
    {

        private TextType containmentProcedureField;

        private TextType cleanUpProcedureField;

        private TextType personalPrecautionHandlingField;

        private TextType environmentalPrecautionHandlingField;

        private TextType neutralizingAgentItemNameField;

        public TextType ContainmentProcedure
        {
            get => this.containmentProcedureField;
            set => this.containmentProcedureField = value;
        }

        public TextType CleanUpProcedure
        {
            get => this.cleanUpProcedureField;
            set => this.cleanUpProcedureField = value;
        }

        public TextType PersonalPrecautionHandling
        {
            get => this.personalPrecautionHandlingField;
            set => this.personalPrecautionHandlingField = value;
        }

        public TextType EnvironmentalPrecautionHandling
        {
            get => this.environmentalPrecautionHandlingField;
            set => this.environmentalPrecautionHandlingField = value;
        }

        public TextType NeutralizingAgentItemName
        {
            get => this.neutralizingAgentItemNameField;
            set => this.neutralizingAgentItemNameField = value;
        }
    }
}