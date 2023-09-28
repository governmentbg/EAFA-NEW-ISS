namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeSettlementPaymentMeans", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeSettlementPaymentMeansType
    {

        private PaymentMeansChannelCodeType paymentChannelCodeField;

        private PaymentMeansCodeType typeCodeField;

        private PaymentGuaranteeMeansCodeType guaranteeMethodCodeField;

        private PaymentMethodCodeType paymentMethodCodeField;

        private IDType[] idField;

        private TextType[] informationField;

        private AmountType paidAmountField;

        private CIDebtorFinancialAccountType payerPartyCIDebtorFinancialAccountField;

        private CICreditorFinancialAccountType payeePartyCICreditorFinancialAccountField;

        private CIDebtorFinancialInstitutionType payerSpecifiedCIDebtorFinancialInstitutionField;

        private CICreditorFinancialInstitutionType payeeSpecifiedCICreditorFinancialInstitutionField;

        private TradeSettlementFinancialCardType applicableTradeSettlementFinancialCardField;

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

        public PaymentMethodCodeType PaymentMethodCode
        {
            get => this.paymentMethodCodeField;
            set => this.paymentMethodCodeField = value;
        }

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Information")]
        public TextType[] Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public AmountType PaidAmount
        {
            get => this.paidAmountField;
            set => this.paidAmountField = value;
        }

        public CIDebtorFinancialAccountType PayerPartyCIDebtorFinancialAccount
        {
            get => this.payerPartyCIDebtorFinancialAccountField;
            set => this.payerPartyCIDebtorFinancialAccountField = value;
        }

        public CICreditorFinancialAccountType PayeePartyCICreditorFinancialAccount
        {
            get => this.payeePartyCICreditorFinancialAccountField;
            set => this.payeePartyCICreditorFinancialAccountField = value;
        }

        public CIDebtorFinancialInstitutionType PayerSpecifiedCIDebtorFinancialInstitution
        {
            get => this.payerSpecifiedCIDebtorFinancialInstitutionField;
            set => this.payerSpecifiedCIDebtorFinancialInstitutionField = value;
        }

        public CICreditorFinancialInstitutionType PayeeSpecifiedCICreditorFinancialInstitution
        {
            get => this.payeeSpecifiedCICreditorFinancialInstitutionField;
            set => this.payeeSpecifiedCICreditorFinancialInstitutionField = value;
        }

        public TradeSettlementFinancialCardType ApplicableTradeSettlementFinancialCard
        {
            get => this.applicableTradeSettlementFinancialCardField;
            set => this.applicableTradeSettlementFinancialCardField = value;
        }
    }
}