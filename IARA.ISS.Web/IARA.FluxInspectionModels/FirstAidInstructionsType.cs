namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FirstAidInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FirstAidInstructionsType
    {

        private TextType medicalConditionExposureResultHandlingField;

        private TextType procedureField;

        private TextType hazardousMaterialEntryRouteTypeField;

        private TextType noteToPhysicianProcedureField;

        public TextType MedicalConditionExposureResultHandling
        {
            get => this.medicalConditionExposureResultHandlingField;
            set => this.medicalConditionExposureResultHandlingField = value;
        }

        public TextType Procedure
        {
            get => this.procedureField;
            set => this.procedureField = value;
        }

        public TextType HazardousMaterialEntryRouteType
        {
            get => this.hazardousMaterialEntryRouteTypeField;
            set => this.hazardousMaterialEntryRouteTypeField = value;
        }

        public TextType NoteToPhysicianProcedure
        {
            get => this.noteToPhysicianProcedureField;
            set => this.noteToPhysicianProcedureField = value;
        }
    }
}