namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RecordedStatus", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RecordedStatusType
    {

        private CodeType conditionCodeField;

        private TextType changerNameField;

        private DateTimeType changedDateTimeField;

        public CodeType ConditionCode
        {
            get
            {
                return this.conditionCodeField;
            }
            set
            {
                this.conditionCodeField = value;
            }
        }

        public TextType ChangerName
        {
            get
            {
                return this.changerNameField;
            }
            set
            {
                this.changerNameField = value;
            }
        }

        public DateTimeType ChangedDateTime
        {
            get
            {
                return this.changedDateTimeField;
            }
            set
            {
                this.changedDateTimeField = value;
            }
        }
    }
}