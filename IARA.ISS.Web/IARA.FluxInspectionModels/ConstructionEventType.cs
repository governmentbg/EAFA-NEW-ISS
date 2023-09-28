namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ConstructionEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ConstructionEventType
    {

        private TextType[] descriptionField;

        private DateTimeType occurrenceDateTimeField;

        private ConstructionLocationType relatedConstructionLocationField;

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

        public ConstructionLocationType RelatedConstructionLocation
        {
            get
            {
                return this.relatedConstructionLocationField;
            }
            set
            {
                this.relatedConstructionLocationField = value;
            }
        }
    }
}