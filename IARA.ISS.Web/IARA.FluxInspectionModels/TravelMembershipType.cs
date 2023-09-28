namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TravelMembership", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TravelMembershipType
    {

        private IDType idField;

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public DateTimeType StartDateTime
        {
            get => this.startDateTimeField;
            set => this.startDateTimeField = value;
        }

        public DateTimeType EndDateTime
        {
            get => this.endDateTimeField;
            set => this.endDateTimeField = value;
        }
    }
}