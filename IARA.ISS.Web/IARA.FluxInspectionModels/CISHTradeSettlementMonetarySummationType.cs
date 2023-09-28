namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISHTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISHTradeSettlementMonetarySummationType
    {

        private AmountType excludingTaxesLineTotalAmountField;

        private AmountType includingTaxesLineTotalAmountField;

        private AmountType totalAllowanceChargeAmountField;

        private AmountType totalDiscountAmountField;

        private AmountType taxTotalAmountField;

        private AmountType taxBasisTotalAmountField;

        public AmountType ExcludingTaxesLineTotalAmount
        {
            get => this.excludingTaxesLineTotalAmountField;
            set => this.excludingTaxesLineTotalAmountField = value;
        }

        public AmountType IncludingTaxesLineTotalAmount
        {
            get => this.includingTaxesLineTotalAmountField;
            set => this.includingTaxesLineTotalAmountField = value;
        }

        public AmountType TotalAllowanceChargeAmount
        {
            get => this.totalAllowanceChargeAmountField;
            set => this.totalAllowanceChargeAmountField = value;
        }

        public AmountType TotalDiscountAmount
        {
            get => this.totalDiscountAmountField;
            set => this.totalDiscountAmountField = value;
        }

        public AmountType TaxTotalAmount
        {
            get => this.taxTotalAmountField;
            set => this.taxTotalAmountField = value;
        }

        public AmountType TaxBasisTotalAmount
        {
            get => this.taxBasisTotalAmountField;
            set => this.taxBasisTotalAmountField = value;
        }
    }
}