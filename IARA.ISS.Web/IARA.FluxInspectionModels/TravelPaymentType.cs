namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TravelPayment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TravelPaymentType
    {

        private IDType instructionIDField;

        private FormattedDateTimeType receivedDateTimeField;

        private AmountType paidAmountField;

        private AmountType taxAmountField;

        private FormattedDateTimeType creationDateTimeField;

        private CodeType priorityCodeField;

        private CodeType statusCodeField;

        private CodeType reasonCodeField;

        private DateTimeType dueDateTimeField;

        private TextType informationField;

        private FinancialGuaranteeType identifiedFinancialGuaranteeField;

        private TradeSettlementPaymentMeansType[] identifiedTradeSettlementPaymentMeansField;

        public IDType InstructionID
        {
            get => this.instructionIDField;
            set => this.instructionIDField = value;
        }

        public FormattedDateTimeType ReceivedDateTime
        {
            get => this.receivedDateTimeField;
            set => this.receivedDateTimeField = value;
        }

        public AmountType PaidAmount
        {
            get => this.paidAmountField;
            set => this.paidAmountField = value;
        }

        public AmountType TaxAmount
        {
            get => this.taxAmountField;
            set => this.taxAmountField = value;
        }

        public FormattedDateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        public CodeType PriorityCode
        {
            get => this.priorityCodeField;
            set => this.priorityCodeField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public CodeType ReasonCode
        {
            get => this.reasonCodeField;
            set => this.reasonCodeField = value;
        }

        public DateTimeType DueDateTime
        {
            get => this.dueDateTimeField;
            set => this.dueDateTimeField = value;
        }

        public TextType Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public FinancialGuaranteeType IdentifiedFinancialGuarantee
        {
            get => this.identifiedFinancialGuaranteeField;
            set => this.identifiedFinancialGuaranteeField = value;
        }

        [XmlElement("IdentifiedTradeSettlementPaymentMeans")]
        public TradeSettlementPaymentMeansType[] IdentifiedTradeSettlementPaymentMeans
        {
            get => this.identifiedTradeSettlementPaymentMeansField;
            set => this.identifiedTradeSettlementPaymentMeansField = value;
        }
    }
}