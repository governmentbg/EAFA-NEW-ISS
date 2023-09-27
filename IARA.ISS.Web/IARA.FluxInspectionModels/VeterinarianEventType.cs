namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VeterinarianEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VeterinarianEventType
    {

        private CodeType typeCodeField;

        private DateTimeType occurrenceDateTimeField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DateTimeType OccurrenceDateTime
        {
            get => this.occurrenceDateTimeField;
            set => this.occurrenceDateTimeField = value;
        }
    }
}