namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRHTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRHTradeSettlementMonetarySummationType
    {

        private AmountType[] equivalentTransferTotalAmountField;

        private AmountType[] paymentTotalAmountField;

        private AmountType[] balanceOutAmountField;

        private AmountType[] adjustedBalanceOutAmountField;

        private AmountType taxTotalAmountField;

        private AmountType includingTaxesLineTotalAmountField;

        private AmountType[] grandTotalAmountField;

        private AmountType[] netLineTotalAmountField;

        private CIRHSpecifiedBalanceOutType[] applicableCIRHSpecifiedBalanceOutField;

        [XmlElement("EquivalentTransferTotalAmount")]
        public AmountType[] EquivalentTransferTotalAmount
        {
            get => this.equivalentTransferTotalAmountField;
            set => this.equivalentTransferTotalAmountField = value;
        }

        [XmlElement("PaymentTotalAmount")]
        public AmountType[] PaymentTotalAmount
        {
            get => this.paymentTotalAmountField;
            set => this.paymentTotalAmountField = value;
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

        [XmlElement("GrandTotalAmount")]
        public AmountType[] GrandTotalAmount
        {
            get => this.grandTotalAmountField;
            set => this.grandTotalAmountField = value;
        }

        [XmlElement("NetLineTotalAmount")]
        public AmountType[] NetLineTotalAmount
        {
            get => this.netLineTotalAmountField;
            set => this.netLineTotalAmountField = value;
        }

        [XmlElement("ApplicableCIRHSpecifiedBalanceOut")]
        public CIRHSpecifiedBalanceOutType[] ApplicableCIRHSpecifiedBalanceOut
        {
            get => this.applicableCIRHSpecifiedBalanceOutField;
            set => this.applicableCIRHSpecifiedBalanceOutField = value;
        }
    }
}