namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTEventType
    {

        private TTLocationType[] occurrenceTTLocationField;

        private DelimitedPeriodType[] occurrenceDelimitedPeriodField;

        [XmlElement("OccurrenceTTLocation")]
        public TTLocationType[] OccurrenceTTLocation
        {
            get
            {
                return this.occurrenceTTLocationField;
            }
            set
            {
                this.occurrenceTTLocationField = value;
            }
        }

        [XmlElement("OccurrenceDelimitedPeriod")]
        public DelimitedPeriodType[] OccurrenceDelimitedPeriod
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
    }
}