namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRTSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRTSupplyChainTradeSettlementType
    {

        private CurrencyCodeType paymentCurrencyCodeField;

        private TextType invoiceIssuerReferenceField;

        private TextType payerReferenceField;

        private DateTimeType closingBookDueDateTimeField;

        private DateTimeType scheduledPaymentDateTimeField;

        private DateTimeType invoiceDateTimeField;

        private CodeType[] creditorReferenceTypeCodeField;

        private CITradePartyType invoiceeCITradePartyField;

        private CITradePartyType invoicerCITradePartyField;

        private CIFinancialAdjustmentType[] specifiedCIFinancialAdjustmentField;

        private CITradeCurrencyExchangeType paymentApplicableCITradeCurrencyExchangeField;

        private CIReferencedDocumentType invoiceReferencedCIReferencedDocumentField;

        private CIRTTradeSettlementMonetarySummationType specifiedCIRTTradeSettlementMonetarySummationField;

        private CITradeTaxType[] applicableCITradeTaxField;

        private CITradeAllowanceChargeType[] specifiedCITradeAllowanceChargeField;

        public CurrencyCodeType PaymentCurrencyCode
        {
            get => this.paymentCurrencyCodeField;
            set => this.paymentCurrencyCodeField = value;
        }

        public TextType InvoiceIssuerReference
        {
            get => this.invoiceIssuerReferenceField;
            set => this.invoiceIssuerReferenceField = value;
        }

        public TextType PayerReference
        {
            get => this.payerReferenceField;
            set => this.payerReferenceField = value;
        }

        public DateTimeType ClosingBookDueDateTime
        {
            get => this.closingBookDueDateTimeField;
            set => this.closingBookDueDateTimeField = value;
        }

        public DateTimeType ScheduledPaymentDateTime
        {
            get => this.scheduledPaymentDateTimeField;
            set => this.scheduledPaymentDateTimeField = value;
        }

        public DateTimeType InvoiceDateTime
        {
            get => this.invoiceDateTimeField;
            set => this.invoiceDateTimeField = value;
        }

        [XmlElement("CreditorReferenceTypeCode")]
        public CodeType[] CreditorReferenceTypeCode
        {
            get => this.creditorReferenceTypeCodeField;
            set => this.creditorReferenceTypeCodeField = value;
        }

        public CITradePartyType InvoiceeCITradeParty
        {
            get => this.invoiceeCITradePartyField;
            set => this.invoiceeCITradePartyField = value;
        }

        public CITradePartyType InvoicerCITradeParty
        {
            get => this.invoicerCITradePartyField;
            set => this.invoicerCITradePartyField = value;
        }

        [XmlElement("SpecifiedCIFinancialAdjustment")]
        public CIFinancialAdjustmentType[] SpecifiedCIFinancialAdjustment
        {
            get => this.specifiedCIFinancialAdjustmentField;
            set => this.specifiedCIFinancialAdjustmentField = value;
        }

        public CITradeCurrencyExchangeType PaymentApplicableCITradeCurrencyExchange
        {
            get => this.paymentApplicableCITradeCurrencyExchangeField;
            set => this.paymentApplicableCITradeCurrencyExchangeField = value;
        }

        public CIReferencedDocumentType InvoiceReferencedCIReferencedDocument
        {
            get => this.invoiceReferencedCIReferencedDocumentField;
            set => this.invoiceReferencedCIReferencedDocumentField = value;
        }

        public CIRTTradeSettlementMonetarySummationType SpecifiedCIRTTradeSettlementMonetarySummation
        {
            get => this.specifiedCIRTTradeSettlementMonetarySummationField;
            set => this.specifiedCIRTTradeSettlementMonetarySummationField = value;
        }

        [XmlElement("ApplicableCITradeTax")]
        public CITradeTaxType[] ApplicableCITradeTax
        {
            get => this.applicableCITradeTaxField;
            set => this.applicableCITradeTaxField = value;
        }

        [XmlElement("SpecifiedCITradeAllowanceCharge")]
        public CITradeAllowanceChargeType[] SpecifiedCITradeAllowanceCharge
        {
            get => this.specifiedCITradeAllowanceChargeField;
            set => this.specifiedCITradeAllowanceChargeField = value;
        }
    }
}