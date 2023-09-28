namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILSupplyChainTradeSettlementType
    {

        private TextType[] paymentReferenceField;

        private TextType invoiceIssuerReferenceField;

        private DateTimeType invoiceDateTimeField;

        private CodeType directionCodeField;

        private CITradeTaxType[] applicableCITradeTaxField;

        private CISpecifiedPeriodType billingCISpecifiedPeriodField;

        private CITradeAllowanceChargeType[] specifiedCITradeAllowanceChargeField;

        private CITradeTaxType[] subtotalCalculatedCITradeTaxField;

        private CILogisticsServiceChargeType[] specifiedCILogisticsServiceChargeField;

        private CITradePaymentTermsType[] specifiedCITradePaymentTermsField;

        private CIILTradeSettlementMonetarySummationType specifiedCIILTradeSettlementMonetarySummationField;

        private CIFinancialAdjustmentType[] specifiedCIFinancialAdjustmentField;

        private CIReferencedDocumentType invoiceReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private TradeSettlementFinancialCardType specifiedTradeSettlementFinancialCardField;

        private CITradeAccountingAccountType[] payableSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType[] receivableSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType[] purchaseSpecifiedCITradeAccountingAccountField;

        private CITradeAccountingAccountType[] salesSpecifiedCITradeAccountingAccountField;

        [XmlElement("PaymentReference")]
        public TextType[] PaymentReference
        {
            get => this.paymentReferenceField;
            set => this.paymentReferenceField = value;
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

        public CodeType DirectionCode
        {
            get => this.directionCodeField;
            set => this.directionCodeField = value;
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

        public CIILTradeSettlementMonetarySummationType SpecifiedCIILTradeSettlementMonetarySummation
        {
            get => this.specifiedCIILTradeSettlementMonetarySummationField;
            set => this.specifiedCIILTradeSettlementMonetarySummationField = value;
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

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get => this.additionalReferencedCIReferencedDocumentField;
            set => this.additionalReferencedCIReferencedDocumentField = value;
        }

        public TradeSettlementFinancialCardType SpecifiedTradeSettlementFinancialCard
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
    }
}