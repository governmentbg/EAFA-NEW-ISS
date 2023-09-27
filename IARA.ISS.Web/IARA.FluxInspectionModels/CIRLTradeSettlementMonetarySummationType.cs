namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRLTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRLTradeSettlementMonetarySummationType
    {

        private AmountType[] duePayableAmountField;

        private AmountType[] paymentTotalAmountField;

        private AmountType taxTotalAmountField;

        private AmountType includingTaxesLineTotalAmountField;

        private AmountType[] netLineTotalAmountField;

        [XmlElement("DuePayableAmount")]
        public AmountType[] DuePayableAmount
        {
            get => this.duePayableAmountField;
            set => this.duePayableAmountField = value;
        }

        [XmlElement("PaymentTotalAmount")]
        public AmountType[] PaymentTotalAmount
        {
            get => this.paymentTotalAmountField;
            set => this.paymentTotalAmountField = value;
        }

        public AmountType TaxTotalAmount
        {
            get => this.taxTotalAmountField;
            set => this.taxTotalAmountField = value;
        }

        public AmountType IncludingTaxesLineTotalAmount
        {
            get => this.includingTaxesLineTotalAmountField;
            set => this.includingTaxesLineTotalAmountField = value;
        }

        [XmlElement("NetLineTotalAmount")]
        public AmountType[] NetLineTotalAmount
        {
            get => this.netLineTotalAmountField;
            set => this.netLineTotalAmountField = value;
        }
    }
}