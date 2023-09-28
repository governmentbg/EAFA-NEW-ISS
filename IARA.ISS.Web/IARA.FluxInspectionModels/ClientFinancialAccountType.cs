namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ClientFinancialAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ClientFinancialAccountType
    {

        private IDType iBANIDField;

        private TextType accountNameField;

        private CodeType currencyCodeField;

        private PaymentFinancialInstitutionType servicingPaymentFinancialInstitutionField;

        public IDType IBANID
        {
            get => this.iBANIDField;
            set => this.iBANIDField = value;
        }

        public TextType AccountName
        {
            get => this.accountNameField;
            set => this.accountNameField = value;
        }

        public CodeType CurrencyCode
        {
            get => this.currencyCodeField;
            set => this.currencyCodeField = value;
        }

        public PaymentFinancialInstitutionType ServicingPaymentFinancialInstitution
        {
            get => this.servicingPaymentFinancialInstitutionField;
            set => this.servicingPaymentFinancialInstitutionField = value;
        }
    }
}