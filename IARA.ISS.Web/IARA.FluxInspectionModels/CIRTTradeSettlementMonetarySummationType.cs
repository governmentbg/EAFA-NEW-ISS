namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRTTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRTTradeSettlementMonetarySummationType
    {

        private AmountType[] duePayableAmountField;

        private AmountType[] grandTotalAmountField;

        private AmountType[] paymentTotalAmountField;

        private AmountType[] totalDiscountAmountField;

        private AmountType[] totalDiscountBasisAmountField;

        private AmountType[] balanceOutAmountField;

        private AmountType[] adjustedBalanceOutAmountField;

        private AmountType taxTotalAmountField;

        private AmountType includingTaxesLineTotalAmountField;

        private AmountType[] netLineTotalAmountField;

        private CIRTSpecifiedBalanceOutType[] applicableCIRTSpecifiedBalanceOutField;

        [XmlElement("DuePayableAmount")]
        public AmountType[] DuePayableAmount
        {
            get => this.duePayableAmountField;
            set => this.duePayableAmountField = value;
        }

        [XmlElement("GrandTotalAmount")]
        public AmountType[] GrandTotalAmount
        {
            get => this.grandTotalAmountField;
            set => this.grandTotalAmountField = value;
        }

        [XmlElement("PaymentTotalAmount")]
        public AmountType[] PaymentTotalAmount
        {
            get => this.paymentTotalAmountField;
            set => this.paymentTotalAmountField = value;
        }

        [XmlElement("TotalDiscountAmount")]
        public AmountType[] TotalDiscountAmount
        {
            get => this.totalDiscountAmountField;
            set => this.totalDiscountAmountField = value;
        }

        [XmlElement("TotalDiscountBasisAmount")]
        public AmountType[] TotalDiscountBasisAmount
        {
            get => this.totalDiscountBasisAmountField;
            set => this.totalDiscountBasisAmountField = value;
        }

        [XmlElement("BalanceOutAmount")]
        public AmountType[] BalanceOutAmount
        {
            get => this.balanceOutAmountField;
            set => this.balanceOutAmountField = value;
        }

        [XmlElement("AdjustedBalanceOutAmount")]
        public AmountType[] AdjustedBalanceOutAmount
        {
            get => this.adjustedBalanceOutAmountField;
            set => this.adjustedBalanceOutAmountField = value;
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

        [XmlElement("ApplicableCIRTSpecifiedBalanceOut")]
        public CIRTSpecifiedBalanceOutType[] ApplicableCIRTSpecifiedBalanceOut
        {
            get => this.applicableCIRTSpecifiedBalanceOutField;
            set => this.applicableCIRTSpecifiedBalanceOutField = value;
        }
    }
}