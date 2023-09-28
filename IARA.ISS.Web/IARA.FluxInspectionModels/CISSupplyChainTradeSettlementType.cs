namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISSupplyChainTradeSettlementType
    {

        private AmountType paymentAmountField;

        private CodeType priceCurrencyCodeField;

        private TextType[] paymentReferenceField;

        private CITradeTaxType[] applicableCITradeTaxField;

        private CITradePaymentTermsType[] specifiedCITradePaymentTermsField;

        public AmountType PaymentAmount
        {
            get => this.paymentAmountField;
            set => this.paymentAmountField = value;
        }

        public CodeType PriceCurrencyCode
        {
            get => this.priceCurrencyCodeField;
            set => this.priceCurrencyCodeField = value;
        }

        [XmlElement("PaymentReference")]
        public TextType[] PaymentReference
        {
            get => this.paymentReferenceField;
            set => this.paymentReferenceField = value;
        }

        [XmlElement("ApplicableCITradeTax")]
        public CITradeTaxType[] ApplicableCITradeTax
        {
            get => this.applicableCITradeTaxField;
            set => this.applicableCITradeTaxField = value;
        }

        [XmlElement("SpecifiedCITradePaymentTerms")]
        public CITradePaymentTermsType[] SpecifiedCITradePaymentTerms
        {
            get => this.specifiedCITradePaymentTermsField;
            set => this.specifiedCITradePaymentTermsField = value;
        }
    }
}