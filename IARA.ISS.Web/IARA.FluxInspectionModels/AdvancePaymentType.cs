namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AdvancePayment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AdvancePaymentType
    {

        private AmountType paidAmountField;

        private string receivedDateTimeField;

        private FormattedDateTimeType formattedReceivedDateTimeField;

        public AmountType PaidAmount
        {
            get
            {
                return this.paidAmountField;
            }
            set
            {
                this.paidAmountField = value;
            }
        }

        public string ReceivedDateTime
        {
            get
            {
                return this.receivedDateTimeField;
            }
            set
            {
                this.receivedDateTimeField = value;
            }
        }

        public FormattedDateTimeType FormattedReceivedDateTime
        {
            get
            {
                return this.formattedReceivedDateTimeField;
            }
            set
            {
                this.formattedReceivedDateTimeField = value;
            }
        }
    }
}