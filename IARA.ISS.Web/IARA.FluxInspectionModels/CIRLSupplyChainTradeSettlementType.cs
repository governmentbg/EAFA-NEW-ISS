namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRLSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRLSupplyChainTradeSettlementType
    {

        private TextType invoiceIssuerReferenceField;

        private TextType payerReferenceField;

        private DateTimeType invoiceDateTimeField;

        private CodeType[] creditorReferenceTypeCodeField;

        private CodeType statusCodeField;

        private CIFinancialAdjustmentType[] specifiedCIFinancialAdjustmentField;

        private CIReferencedDocumentType invoiceReferencedCIReferencedDocumentField;

        private CIRLTradeSettlementMonetarySummationType specifiedCIRLTradeSettlementMonetarySummationField;

        private CIRDocumentLineDocumentType associatedCIRDocumentLineDocumentField;

        private CIReferencedDocumentType[] referencedCIReferencedDocumentField;

        private CIRLSupplyChainTradeTransactionType referencedCIRLSupplyChainTradeTransactionField;

        private CITradeTaxType[] applicableCITradeTaxField;

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

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
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

        public CIRLTradeSettlementMonetarySummationType SpecifiedCIRLTradeSettlementMonetarySummation
        {
            get => this.specifiedCIRLTradeSettlementMonetarySummationField;
            set => this.specifiedCIRLTradeSettlementMonetarySummationField = value;
        }

        public CIRDocumentLineDocumentType AssociatedCIRDocumentLineDocument
        {
            get => this.associatedCIRDocumentLineDocumentField;
            set => this.associatedCIRDocumentLineDocumentField = value;
        }

        [XmlElement("ReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] ReferencedCIReferencedDocument
        {
            get => this.referencedCIReferencedDocumentField;
            set => this.referencedCIReferencedDocumentField = value;
        }

        public CIRLSupplyChainTradeTransactionType ReferencedCIRLSupplyChainTradeTransaction
        {
            get => this.referencedCIRLSupplyChainTradeTransactionField;
            set => this.referencedCIRLSupplyChainTradeTransactionField = value;
        }

        [XmlElement("ApplicableCITradeTax")]
        public CITradeTaxType[] ApplicableCITradeTax
        {
            get => this.applicableCITradeTaxField;
            set => this.applicableCITradeTaxField = value;
        }
    }
}