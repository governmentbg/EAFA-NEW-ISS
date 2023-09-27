namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectScheduleTaskRelationship", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectScheduleTaskRelationshipType
    {

        private IDType id;

        private TextType nameField;

        private TextType descriptionField;

        private ScheduleTaskRelationshipTypeCodeType typeCodeField;

        private DurationUnitMeasureType lagTimeMeasureField;

        private PercentType lagTimePercentField;

        private ProjectScheduleTaskType[] successorSpecifiedProjectScheduleTaskField;

        private ProjectScheduleTaskType[] predecessorSpecifiedProjectScheduleTaskField;

        private ProjectScheduleCalendarType lagTimeProjectScheduleCalendarField;

        public IDType ID
        {
            get => this.id;
            set => this.id = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public ScheduleTaskRelationshipTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DurationUnitMeasureType LagTimeMeasure
        {
            get => this.lagTimeMeasureField;
            set => this.lagTimeMeasureField = value;
        }

        public PercentType LagTimePercent
        {
            get => this.lagTimePercentField;
            set => this.lagTimePercentField = value;
        }

        [XmlElement("SuccessorSpecifiedProjectScheduleTask")]
        public ProjectScheduleTaskType[] SuccessorSpecifiedProjectScheduleTask
        {
            get => this.successorSpecifiedProjectScheduleTaskField;
            set => this.successorSpecifiedProjectScheduleTaskField = value;
        }

        [XmlElement("PredecessorSpecifiedProjectScheduleTask")]
        public ProjectScheduleTaskType[] PredecessorSpecifiedProjectScheduleTask
        {
            get => this.predecessorSpecifiedProjectScheduleTaskField;
            set => this.predecessorSpecifiedProjectScheduleTaskField = value;
        }

        public ProjectScheduleCalendarType LagTimeProjectScheduleCalendar
        {
            get => this.lagTimeProjectScheduleCalendarField;
            set => this.lagTimeProjectScheduleCalendarField = value;
        }
    }
}