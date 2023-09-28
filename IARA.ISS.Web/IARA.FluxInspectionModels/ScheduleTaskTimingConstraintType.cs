namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ScheduleTaskTimingConstraint", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ScheduleTaskTimingConstraintType
    {

        private DateTimeType limitDateTimeField;

        private CodeType typeCodeField;

        public DateTimeType LimitDateTime
        {
            get => this.limitDateTimeField;
            set => this.limitDateTimeField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }
    }
}