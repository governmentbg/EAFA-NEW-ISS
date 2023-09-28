namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIFinancialAdjustment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIFinancialAdjustmentType
    {

        private FinancialAdjustmentReasonCodeType reasonCodeField;

        private TextType[] reasonField;

        private AmountType[] actualAmountField;

        private QuantityType actualQuantityField;

        private DateTimeType actualDateTimeField;

        private AccountingDebitCreditStatusCodeType directionCodeField;

        private CITradePartyType claimRelatedCITradePartyField;

        private CIReferencedDocumentType invoiceReferenceCIReferencedDocumentField;

        private CITradeTaxType[] relatedCITradeTaxField;

        public FinancialAdjustmentReasonCodeType ReasonCode
        {
            get => this.reasonCodeField;
            set => this.reasonCodeField = value;
        }

        [XmlElement("Reason")]
        public TextType[] Reason
        {
            get => this.reasonField;
            set => this.reasonField = value;
        }

        [XmlElement("ActualAmount")]
        public AmountType[] ActualAmount
        {
            get => this.actualAmountField;
            set => this.actualAmountField = value;
        }

        public QuantityType ActualQuantity
        {
            get => this.actualQuantityField;
            set => this.actualQuantityField = value;
        }

        public DateTimeType ActualDateTime
        {
            get => this.actualDateTimeField;
            set => this.actualDateTimeField = value;
        }

        public AccountingDebitCreditStatusCodeType DirectionCode
        {
            get => this.directionCodeField;
            set => this.directionCodeField = value;
        }

        public CITradePartyType ClaimRelatedCITradeParty
        {
            get => this.claimRelatedCITradePartyField;
            set => this.claimRelatedCITradePartyField = value;
        }

        public CIReferencedDocumentType InvoiceReferenceCIReferencedDocument
        {
            get => this.invoiceReferenceCIReferencedDocumentField;
            set => this.invoiceReferenceCIReferencedDocumentField = value;
        }

        [XmlElement("RelatedCITradeTax")]
        public CITradeTaxType[] RelatedCITradeTax
        {
            get => this.relatedCITradeTaxField;
            set => this.relatedCITradeTaxField = value;
        }
    }
}