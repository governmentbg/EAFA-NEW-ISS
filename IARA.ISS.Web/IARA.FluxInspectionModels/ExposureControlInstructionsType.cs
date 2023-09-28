namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ExposureControlInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ExposureControlInstructionsType
    {

        private TextType[] ventilationHandlingField;

        private TextType workHygienicRecommendedProcedureField;

        [XmlElement("VentilationHandling")]
        public TextType[] VentilationHandling
        {
            get => this.ventilationHandlingField;
            set => this.ventilationHandlingField = value;
        }

        public TextType WorkHygienicRecommendedProcedure
        {
            get => this.workHygienicRecommendedProcedureField;
            set => this.workHygienicRecommendedProcedureField = value;
        }
    }
}