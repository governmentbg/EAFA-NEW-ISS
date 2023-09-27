namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISupplyChainSchedule", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISupplyChainScheduleType
    {

        private TextType[] descriptionField;

        private IDType idField;

        private DateTimeType[] occurrenceDateTimeField;

        private CodeType[] typeCodeField;

        private CodeType[] statusCodeField;

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("OccurrenceDateTime")]
        public DateTimeType[] OccurrenceDateTime
        {
            get => this.occurrenceDateTimeField;
            set => this.occurrenceDateTimeField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("StatusCode")]
        public CodeType[] StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }
    }
}