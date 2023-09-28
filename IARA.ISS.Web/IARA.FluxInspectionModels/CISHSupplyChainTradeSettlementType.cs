namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISHSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISHSupplyChainTradeSettlementType
    {

        private TextType[] descriptionField;

        private IndicatorType discountIndicatorField;

        private CodeType invoiceCurrencyCodeField;

        private CodeType orderCurrencyCodeField;

        private AmountType totalInvoiceAmountField;

        private AmountType totalAdjustmentAmountField;

        private CISHTradeSettlementMonetarySummationType specifiedCISHTradeSettlementMonetarySummationField;

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public IndicatorType DiscountIndicator
        {
            get => this.discountIndicatorField;
            set => this.discountIndicatorField = value;
        }

        public CodeType InvoiceCurrencyCode
        {
            get => this.invoiceCurrencyCodeField;
            set => this.invoiceCurrencyCodeField = value;
        }

        public CodeType OrderCurrencyCode
        {
            get => this.orderCurrencyCodeField;
            set => this.orderCurrencyCodeField = value;
        }

        public AmountType TotalInvoiceAmount
        {
            get => this.totalInvoiceAmountField;
            set => this.totalInvoiceAmountField = value;
        }

        public AmountType TotalAdjustmentAmount
        {
            get => this.totalAdjustmentAmountField;
            set => this.totalAdjustmentAmountField = value;
        }

        public CISHTradeSettlementMonetarySummationType SpecifiedCISHTradeSettlementMonetarySummation
        {
            get => this.specifiedCISHTradeSettlementMonetarySummationField;
            set => this.specifiedCISHTradeSettlementMonetarySummationField = value;
        }
    }
}