namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectScheduleCalendar", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectScheduleCalendarType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private DateType[] holidayDateField;

        private ProjectWorkShiftType[] associatedProjectWorkShiftField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
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

        [XmlElement("HolidayDate")]
        public DateType[] HolidayDate
        {
            get => this.holidayDateField;
            set => this.holidayDateField = value;
        }

        [XmlElement("AssociatedProjectWorkShift")]
        public ProjectWorkShiftType[] AssociatedProjectWorkShift
        {
            get => this.associatedProjectWorkShiftField;
            set => this.associatedProjectWorkShiftField = value;
        }
    }
}