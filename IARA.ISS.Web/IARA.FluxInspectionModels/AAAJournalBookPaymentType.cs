namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalBookPayment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalBookPaymentType
    {

        private AmountType paidAmountField;

        private DateTimeType creationDateTimeField;

        private PaymentMeansCodeType typeCodeField;

        private IDType idField;

        private TextType informationField;

        private AAAJournalBookFinancialAccountType specifiedAAAJournalBookFinancialAccountField;

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

        public DateTimeType CreationDateTime
        {
            get
            {
                return this.creationDateTimeField;
            }
            set
            {
                this.creationDateTimeField = value;
            }
        }

        public PaymentMeansCodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TextType Information
        {
            get
            {
                return this.informationField;
            }
            set
            {
                this.informationField = value;
            }
        }

        public AAAJournalBookFinancialAccountType SpecifiedAAAJournalBookFinancialAccount
        {
            get
            {
                return this.specifiedAAAJournalBookFinancialAccountField;
            }
            set
            {
                this.specifiedAAAJournalBookFinancialAccountField = value;
            }
        }
    }
}