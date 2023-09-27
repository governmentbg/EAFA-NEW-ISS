namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAEntryMonetaryInstalment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAEntryMonetaryInstalmentType
    {

        private AmountType paymentAmountField;

        private DateTimeType dueDateDateTimeField;

        private NumericType rankingNumericField;

        public AmountType PaymentAmount
        {
            get
            {
                return this.paymentAmountField;
            }
            set
            {
                this.paymentAmountField = value;
            }
        }

        public DateTimeType DueDateDateTime
        {
            get
            {
                return this.dueDateDateTimeField;
            }
            set
            {
                this.dueDateDateTimeField = value;
            }
        }

        public NumericType RankingNumeric
        {
            get
            {
                return this.rankingNumericField;
            }
            set
            {
                this.rankingNumericField = value;
            }
        }
    }
}