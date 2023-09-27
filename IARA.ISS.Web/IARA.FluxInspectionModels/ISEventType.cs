namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ISEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ISEventType
    {

        private IDType idField;

        private TextType[] descriptionField;

        private DateTimeType occurrenceDateTimeField;

        private CodeType typeCodeField;

        private DelimitedPeriodType occurrenceDelimitedPeriodField;

        private ISEventType[] subordinateISEventField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
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

        public DateTimeType OccurrenceDateTime
        {
            get
            {
                return this.occurrenceDateTimeField;
            }
            set
            {
                this.occurrenceDateTimeField = value;
            }
        }

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public DelimitedPeriodType OccurrenceDelimitedPeriod
        {
            get
            {
                return this.occurrenceDelimitedPeriodField;
            }
            set
            {
                this.occurrenceDelimitedPeriodField = value;
            }
        }

        [XmlElement("SubordinateISEvent")]
        public ISEventType[] SubordinateISEvent
        {
            get
            {
                return this.subordinateISEventField;
            }
            set
            {
                this.subordinateISEventField = value;
            }
        }
    }
}