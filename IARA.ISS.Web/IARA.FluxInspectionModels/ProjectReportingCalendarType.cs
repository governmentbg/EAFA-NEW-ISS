namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectReportingCalendar", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectReportingCalendarType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private DateType endDateField;

        private ProjectPeriodType[] specifiedProjectPeriodField;

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

        public DateType EndDate
        {
            get => this.endDateField;
            set => this.endDateField = value;
        }

        [XmlElement("SpecifiedProjectPeriod")]
        public ProjectPeriodType[] SpecifiedProjectPeriod
        {
            get => this.specifiedProjectPeriodField;
            set => this.specifiedProjectPeriodField = value;
        }
    }
}