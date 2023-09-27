namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnimalExaminationResult", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnimalExaminationResultType
    {

        private DateTimeType approvalDateTimeField;

        private TextType[] statementField;

        public DateTimeType ApprovalDateTime
        {
            get
            {
                return this.approvalDateTimeField;
            }
            set
            {
                this.approvalDateTimeField = value;
            }
        }

        [XmlElement("Statement")]
        public TextType[] Statement
        {
            get
            {
                return this.statementField;
            }
            set
            {
                this.statementField = value;
            }
        }
    }
}