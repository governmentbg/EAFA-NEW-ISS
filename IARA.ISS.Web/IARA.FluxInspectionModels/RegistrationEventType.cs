namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RegistrationEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RegistrationEventType
    {

        private TextType[] descriptionField;

        private DateTimeType occurrenceDateTimeField;

        private RegistrationLocationType relatedRegistrationLocationField;

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public DateTimeType OccurrenceDateTime
        {
            get => this.occurrenceDateTimeField;
            set => this.occurrenceDateTimeField = value;
        }

        public RegistrationLocationType RelatedRegistrationLocation
        {
            get => this.relatedRegistrationLocationField;
            set => this.relatedRegistrationLocationField = value;
        }
    }
}