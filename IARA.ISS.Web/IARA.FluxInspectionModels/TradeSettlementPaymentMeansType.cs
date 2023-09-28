namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TradeSettlementPaymentMeans", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TradeSettlementPaymentMeansType
    {

        private PaymentMeansChannelCodeType paymentChannelCodeField;

        private PaymentMeansCodeType typeCodeField;

        private PaymentGuaranteeMeansCodeType guaranteeMethodCodeField;

        private CodeType paymentMethodCodeField;

        private TextType typeField;

        private TextType[] informationField;

        private IDType[] idField;

        private AmountType paidAmountField;

        private TradeSettlementFinancialCardType[] applicableTradeSettlementFinancialCardField;

        private DebtorFinancialAccountType payerPartyDebtorFinancialAccountField;

        private CreditorFinancialAccountType[] payeePartyCreditorFinancialAccountField;

        private DebtorFinancialInstitutionType payerSpecifiedDebtorFinancialInstitutionField;

        private CreditorFinancialInstitutionType payeeSpecifiedCreditorFinancialInstitutionField;

        private TradeSettlementFinancialCardType[] identifiedTradeSettlementFinancialCardField;

        private TravelVoucherType[] identifiedTravelVoucherField;

        public PaymentMeansChannelCodeType PaymentChannelCode
        {
            get => this.paymentChannelCodeField;
            set => this.paymentChannelCodeField = value;
        }

        public PaymentMeansCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public PaymentGuaranteeMeansCodeType GuaranteeMethodCode
        {
            get => this.guaranteeMethodCodeField;
            set => this.guaranteeMethodCodeField = value;
        }

        public CodeType PaymentMethodCode
        {
            get => this.paymentMethodCodeField;
            set => this.paymentMethodCodeField = value;
        }

        public TextType Type
        {
            get => this.typeField;
            set => this.typeField = value;
        }

        [XmlElement("Information")]
        public TextType[] Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public AmountType PaidAmount
        {
            get => this.paidAmountField;
            set => this.paidAmountField = value;
        }

        [XmlElement("ApplicableTradeSettlementFinancialCard")]
        public TradeSettlementFinancialCardType[] ApplicableTradeSettlementFinancialCard
        {
            get => this.applicableTradeSettlementFinancialCardField;
            set => this.applicableTradeSettlementFinancialCardField = value;
        }

        public DebtorFinancialAccountType PayerPartyDebtorFinancialAccount
        {
            get => this.payerPartyDebtorFinancialAccountField;
            set => this.payerPartyDebtorFinancialAccountField = value;
        }

        [XmlElement("PayeePartyCreditorFinancialAccount")]
        public CreditorFinancialAccountType[] PayeePartyCreditorFinancialAccount
        {
            get => this.payeePartyCreditorFinancialAccountField;
            set => this.payeePartyCreditorFinancialAccountField = value;
        }

        public DebtorFinancialInstitutionType PayerSpecifiedDebtorFinancialInstitution
        {
            get => this.payerSpecifiedDebtorFinancialInstitutionField;
            set => this.payerSpecifiedDebtorFinancialInstitutionField = value;
        }

        public CreditorFinancialInstitutionType PayeeSpecifiedCreditorFinancialInstitution
        {
            get => this.payeeSpecifiedCreditorFinancialInstitutionField;
            set => this.payeeSpecifiedCreditorFinancialInstitutionField = value;
        }

        [XmlElement("IdentifiedTradeSettlementFinancialCard")]
        public TradeSettlementFinancialCardType[] IdentifiedTradeSettlementFinancialCard
        {
            get => this.identifiedTradeSettlementFinancialCardField;
            set => this.identifiedTradeSettlementFinancialCardField = value;
        }

        [XmlElement("IdentifiedTravelVoucher")]
        public TravelVoucherType[] IdentifiedTravelVoucher
        {
            get => this.identifiedTravelVoucherField;
            set => this.identifiedTravelVoucherField = value;
        }
    }
}