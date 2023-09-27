namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnimalInspection", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnimalInspectionType
    {

        private SpecifiedPartyType executionSpecifiedPartyField;

        private InspectionPersonType executionInspectionPersonField;

        private InspectionEventType specifiedInspectionEventField;

        private AnimalExaminationResultType specifiedAnimalExaminationResultField;

        public SpecifiedPartyType ExecutionSpecifiedParty
        {
            get
            {
                return this.executionSpecifiedPartyField;
            }
            set
            {
                this.executionSpecifiedPartyField = value;
            }
        }

        public InspectionPersonType ExecutionInspectionPerson
        {
            get
            {
                return this.executionInspectionPersonField;
            }
            set
            {
                this.executionInspectionPersonField = value;
            }
        }

        public InspectionEventType SpecifiedInspectionEvent
        {
            get
            {
                return this.specifiedInspectionEventField;
            }
            set
            {
                this.specifiedInspectionEventField = value;
            }
        }

        public AnimalExaminationResultType SpecifiedAnimalExaminationResult
        {
            get
            {
                return this.specifiedAnimalExaminationResultField;
            }
            set
            {
                this.specifiedAnimalExaminationResultField = value;
            }
        }
    }
}