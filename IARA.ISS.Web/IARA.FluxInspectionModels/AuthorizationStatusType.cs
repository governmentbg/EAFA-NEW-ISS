namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AuthorizationStatus", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AuthorizationStatusType
    {

        private CodeType conditionCodeField;

        private DateTimeType changedDateTimeField;

        private TextType[] descriptionField;

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
    }
}