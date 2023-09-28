namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIIHSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIIHSupplyChainTradeSettlementType
    {

        private AmountType[] duePayableAmountField;

        private IDType creditorReferenceIDField;

        private TextType[] paymentReferenceField;

        private CurrencyCodeType taxCurrencyCodeField;

        private CurrencyCodeType invoiceCurrencyCodeField;

        private CurrencyCodeType paymentCurrencyCodeField;

        private CodeType[] creditorReferenceTypeCodeField;

        private TextType[] creditorReferenceTypeField;

        private IDType[] creditorReferenceIssuerIDField;

        private TextType invoiceIssuerReferenceField;

        private DateTimeType invoiceDateTimeField;

        private DateTimeType[] nextInvoiceDateTimeField;

        private CodeType creditReasonCodeField;

        private TextType[] creditReasonField;

        private CITradePartyType invoicerCITradePartyField;

        private CITradePartyType invoiceeCITradePartyField;

        private CITradePartyType payeeCITradePartyField;

        private CITradePartyType payerCITradePartyField;

        private CITradeCurrencyExchangeType taxApplicableCITradeCurrencyExchangeField;

        private CITradeCurrencyExchangeType invoiceApplicableCITradeCurrencyExchangeField;

        private CITradeCurrencyExchangeType paymentApplicableCITradeCurrencyExchangeField;

        private CITradeSettlementPaymentMeansType[] specifiedCITradeSettlementPaymentMeansField;

        private CITradeTaxType[] applicableCITradeTaxField;

        private CISpecifiedPeriodType billingCISpecifiedPeriodField;

        private CITradeAllowanceChargeType[] specifiedCITradeAllowanceChargeField;

        private CITradeTaxType[] subtotalCalculatedCITradeTaxField;

        private CILogisticsServiceChargeType[] specifiedCILogisticsServiceChargeField;

        private CITradePaymentTermsType[] specifiedCITradePaymentTermsField;

        private CIIHTradeSettlementMonetarySummationType specifiedCIIHTradeSettlementMonetarySummationField;

        private CIFinancialAdjustmentType[] specifiedCIFinancialAdjustmentField;

        private CIReferencedDocumentType invoiceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType proFormaInvoiceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType letterOfCreditReferencedCIReferencedDocumentField;

        private TradeSettlementFinancialCardType[] specifiedTradeSettlementFinancialCardField;

        private CITradeAccountingAccountType[] payableSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType[] receivableSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType[] purchaseSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType[] salesSpecifiedCITradeAccountingAccountField;

        private CIReferencedDocumentType[] factoringAgreementReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] factoringListReferencedCIReferencedDocumentField;

        private CITradePartyType ultimatePayeeCITradePartyField;

        private AdvancePaymentType[] specifiedAdvancePaymentField;

        private CIIHTradeSettlementMonetarySummationType outstandingSpecifiedCIIHTradeSettlementMonetarySummationField;

        [XmlElement("DuePayableAmount")]
        public AmountType[] DuePayableAmount
        {
            get => this.duePayableAmountField;
            set => this.duePayableAmountField = value;
        }

        public IDType CreditorReferenceID
        {
            get => this.creditorReferenceIDField;
            set => this.creditorReferenceIDField = value;
        }

        [XmlElement("PaymentReference")]
        public TextType[] PaymentReference
        {
            get => this.paymentReferenceField;
            set => this.paymentReferenceField = value;
        }

        public CurrencyCodeType TaxCurrencyCode
        {
            get => this.taxCurrencyCodeField;
            set => this.taxCurrencyCodeField = value;
        }

        public CurrencyCodeType InvoiceCurrencyCode
        {
            get => this.invoiceCurrencyCodeField;
            set => this.invoiceCurrencyCodeField = value;
        }

        public CurrencyCodeType PaymentCurrencyCode
        {
            get => this.paymentCurrencyCodeField;
            set => this.paymentCurrencyCodeField = value;
        }

        [XmlElement("CreditorReferenceTypeCode")]
        public CodeType[] CreditorReferenceTypeCode
        {
            get => this.creditorReferenceTypeCodeField;
            set => this.creditorReferenceTypeCodeField = value;
        }

        [XmlElement("CreditorReferenceType")]
        public TextType[] CreditorReferenceType
        {
            get => this.creditorReferenceTypeField;
            set => this.creditorReferenceTypeField = value;
        }

        [XmlElement("CreditorReferenceIssuerID")]
        public IDType[] CreditorReferenceIssuerID
        {
            get => this.creditorReferenceIssuerIDField;
            set => this.creditorReferenceIssuerIDField = value;
        }

        public TextType InvoiceIssuerReference
        {
            get => this.invoiceIssuerReferenceField;
            set => this.invoiceIssuerReferenceField = value;
        }

        public DateTimeType InvoiceDateTime
        {
            get => this.invoiceDateTimeField;
            set => this.invoiceDateTimeField = value;
        }

        [XmlElement("NextInvoiceDateTime")]
        public DateTimeType[] NextInvoiceDateTime
        {
            get => this.nextInvoiceDateTimeField;
            set => this.nextInvoiceDateTimeField = value;
        }

        public CodeType CreditReasonCode
        {
            get => this.creditReasonCodeField;
            set => this.creditReasonCodeField = value;
        }

        [XmlElement("CreditReason")]
        public TextType[] CreditReason
        {
            get => this.creditReasonField;
            set => this.creditReasonField = value;
        }

        public CITradePartyType InvoicerCITradeParty
        {
            get => this.invoicerCITradePartyField;
            set => this.invoicerCITradePartyField = value;
        }

        public CITradePartyType InvoiceeCITradeParty
        {
            get => this.invoiceeCITradePartyField;
            set => this.invoiceeCITradePartyField = value;
        }

        public CITradePartyType PayeeCITradeParty
        {
            get => this.payeeCITradePartyField;
            set => this.payeeCITradePartyField = value;
        }

        public CITradePartyType PayerCITradeParty
        {
            get => this.payerCITradePartyField;
            set => this.payerCITradePartyField = value;
        }

        public CITradeCurrencyExchangeType TaxApplicableCITradeCurrencyExchange
        {
            get => this.taxApplicableCITradeCurrencyExchangeField;
            set => this.taxApplicableCITradeCurrencyExchangeField = value;
        }

        public CITradeCurrencyExchangeType InvoiceApplicableCITradeCurrencyExchange
        {
            get => this.invoiceApplicableCITradeCurrencyExchangeField;
            set => this.invoiceApplicableCITradeCurrencyExchangeField = value;
        }

        public CITradeCurrencyExchangeType PaymentApplicableCITradeCurrencyExchange
        {
            get => this.paymentApplicableCITradeCurrencyExchangeField;
            set => this.paymentApplicableCITradeCurrencyExchangeField = value;
        }

        [XmlElement("SpecifiedCITradeSettlementPaymentMeans")]
        public CITradeSettlementPaymentMeansType[] SpecifiedCITradeSettlementPaymentMeans
        {
            get => this.specifiedCITradeSettlementPaymentMeansField;
            set => this.specifiedCITradeSettlementPaymentMeansField = value;
        }

        [XmlElement("ApplicableCITradeTax")]
        public CITradeTaxType[] ApplicableCITradeTax
        {
            get => this.applicableCITradeTaxField;
            set => this.applicableCITradeTaxField = value;
        }

        public CISpecifiedPeriodType BillingCISpecifiedPeriod
        {
            get => this.billingCISpecifiedPeriodField;
            set => this.billingCISpecifiedPeriodField = value;
        }

        [XmlElement("SpecifiedCITradeAllowanceCharge")]
        public CITradeAllowanceChargeType[] SpecifiedCITradeAllowanceCharge
        {
            get => this.specifiedCITradeAllowanceChargeField;
            set => this.specifiedCITradeAllowanceChargeField = value;
        }

        [XmlElement("SubtotalCalculatedCITradeTax")]
        public CITradeTaxType[] SubtotalCalculatedCITradeTax
        {
            get => this.subtotalCalculatedCITradeTaxField;
            set => this.subtotalCalculatedCITradeTaxField = value;
        }

        [XmlElement("SpecifiedCILogisticsServiceCharge")]
        public CILogisticsServiceChargeType[] SpecifiedCILogisticsServiceCharge
        {
            get => this.specifiedCILogisticsServiceChargeField;
            set => this.specifiedCILogisticsServiceChargeField = value;
        }

        [XmlElement("SpecifiedCITradePaymentTerms")]
        public CITradePaymentTermsType[] SpecifiedCITradePaymentTerms
        {
            get => this.specifiedCITradePaymentTermsField;
            set => this.specifiedCITradePaymentTermsField = value;
        }

        public CIIHTradeSettlementMonetarySummationType SpecifiedCIIHTradeSettlementMonetarySummation
        {
            get => this.specifiedCIIHTradeSettlementMonetarySummationField;
            set => this.specifiedCIIHTradeSettlementMonetarySummationField = value;
        }

        [XmlElement("SpecifiedCIFinancialAdjustment")]
        public CIFinancialAdjustmentType[] SpecifiedCIFinancialAdjustment
        {
            get => this.specifiedCIFinancialAdjustmentField;
            set => this.specifiedCIFinancialAdjustmentField = value;
        }

        public CIReferencedDocumentType InvoiceReferencedCIReferencedDocument
        {
            get => this.invoiceReferencedCIReferencedDocumentField;
            set => this.invoiceReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType ProFormaInvoiceReferencedCIReferencedDocument
        {
            get => this.proFormaInvoiceReferencedCIReferencedDocumentField;
            set => this.proFormaInvoiceReferencedCIReferencedDocumentField = value;
        }

        public CIReferencedDocumentType LetterOfCreditReferencedCIReferencedDocument
        {
            get => this.letterOfCreditReferencedCIReferencedDocumentField;
            set => this.letterOfCreditReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("SpecifiedTradeSettlementFinancialCard")]
        public TradeSettlementFinancialCardType[] SpecifiedTradeSettlementFinancialCard
        {
            get => this.specifiedTradeSettlementFinancialCardField;
            set => this.specifiedTradeSettlementFinancialCardField = value;
        }

        [XmlElement("PayableSpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] PayableSpecifiedCITradeAccountingAccount
        {
            get => this.payableSpecifiedCITradeAccountingAccountField;
            set => this.payableSpecifiedCITradeAccountingAccountField = value;
        }

        [XmlElement("ReceivableSpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] ReceivableSpecifiedCITradeAccountingAccount
        {
            get => this.receivableSpecifiedCITradeAccountingAccountField;
            set => this.receivableSpecifiedCITradeAccountingAccountField = value;
        }

        [XmlElement("PurchaseSpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] PurchaseSpecifiedCITradeAccountingAccount
        {
            get => this.purchaseSpecifiedCITradeAccountingAccountField;
            set => this.purchaseSpecifiedCITradeAccountingAccountField = value;
        }

        [XmlElement("SalesSpecifiedCITradeAccountingAccount")]
        public CITradeAccountingAccountType[] SalesSpecifiedCITradeAccountingAccount
        {
            get => this.salesSpecifiedCITradeAccountingAccountField;
            set => this.salesSpecifiedCITradeAccountingAccountField = value;
        }

        [XmlElement("FactoringAgreementReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] FactoringAgreementReferencedCIReferencedDocument
        {
            get => this.factoringAgreementReferencedCIReferencedDocumentField;
            set => this.factoringAgreementReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("FactoringListReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] FactoringListReferencedCIReferencedDocument
        {
            get => this.factoringListReferencedCIReferencedDocumentField;
            set => this.factoringListReferencedCIReferencedDocumentField = value;
        }

        public CITradePartyType UltimatePayeeCITradeParty
        {
            get => this.ultimatePayeeCITradePartyField;
            set => this.ultimatePayeeCITradePartyField = value;
        }

        [XmlElement("SpecifiedAdvancePayment")]
        public AdvancePaymentType[] SpecifiedAdvancePayment
        {
            get => this.specifiedAdvancePaymentField;
            set => this.specifiedAdvancePaymentField = value;
        }

        public CIIHTradeSettlementMonetarySummationType OutstandingSpecifiedCIIHTradeSettlementMonetarySummation
        {
            get => this.outstandingSpecifiedCIIHTradeSettlementMonetarySummationField;
            set => this.outstandingSpecifiedCIIHTradeSettlementMonetarySummationField = value;
        }
    }
}