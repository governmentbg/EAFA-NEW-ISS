namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("InspectionEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class InspectionEventType
    {

        private DateTimeType occurrenceDateTimeField;

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private ReferencedLocationType occurrenceReferencedLocationField;

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

        public ReferencedLocationType OccurrenceReferencedLocation
        {
            get
            {
                return this.occurrenceReferencedLocationField;
            }
            set
            {
                this.occurrenceReferencedLocationField = value;
            }
        }
    }
}