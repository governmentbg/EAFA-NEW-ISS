namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TradeSettlementFinancialCard", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TradeSettlementFinancialCardType
    {

        private IndicatorType microchipIndicatorField;

        private IDType idField;

        private CodeType typeCodeField;

        private TextType cardholderNameField;

        private DateType expiryDateField;

        private NumericType verificationNumericField;

        private DateOnlyFormattedDateTimeType validFromDateTimeField;

        private AmountType[] creditLimitAmountField;

        private AmountType[] creditAvailableAmountField;

        private PercentType interestRatePercentField;

        private TextType[] issuingCompanyNameField;

        private TextType[] descriptionField;

        public IndicatorType MicrochipIndicator
        {
            get => this.microchipIndicatorField;
            set => this.microchipIndicatorField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType CardholderName
        {
            get => this.cardholderNameField;
            set => this.cardholderNameField = value;
        }

        public DateType ExpiryDate
        {
            get => this.expiryDateField;
            set => this.expiryDateField = value;
        }

        public NumericType VerificationNumeric
        {
            get => this.verificationNumericField;
            set => this.verificationNumericField = value;
        }

        public DateOnlyFormattedDateTimeType ValidFromDateTime
        {
            get => this.validFromDateTimeField;
            set => this.validFromDateTimeField = value;
        }

        [XmlElement("CreditLimitAmount")]
        public AmountType[] CreditLimitAmount
        {
            get => this.creditLimitAmountField;
            set => this.creditLimitAmountField = value;
        }

        [XmlElement("CreditAvailableAmount")]
        public AmountType[] CreditAvailableAmount
        {
            get => this.creditAvailableAmountField;
            set => this.creditAvailableAmountField = value;
        }

        public PercentType InterestRatePercent
        {
            get => this.interestRatePercentField;
            set => this.interestRatePercentField = value;
        }

        [XmlElement("IssuingCompanyName")]
        public TextType[] IssuingCompanyName
        {
            get => this.issuingCompanyNameField;
            set => this.issuingCompanyNameField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}