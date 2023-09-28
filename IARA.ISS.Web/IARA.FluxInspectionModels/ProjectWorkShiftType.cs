namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectWorkShift", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectWorkShiftType
    {

        private IDType idField;

        private CodeType startDayCodeField;

        private CodeType[] workDayCodeField;

        private TextType nameField;

        private ProjectPeriodType effectiveProjectPeriodField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType StartDayCode
        {
            get => this.startDayCodeField;
            set => this.startDayCodeField = value;
        }

        [XmlElement("WorkDayCode")]
        public CodeType[] WorkDayCode
        {
            get => this.workDayCodeField;
            set => this.workDayCodeField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public ProjectPeriodType EffectiveProjectPeriod
        {
            get => this.effectiveProjectPeriodField;
            set => this.effectiveProjectPeriodField = value;
        }
    }
}