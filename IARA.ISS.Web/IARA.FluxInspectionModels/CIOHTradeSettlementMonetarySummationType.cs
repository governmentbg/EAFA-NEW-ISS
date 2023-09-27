namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOHTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOHTradeSettlementMonetarySummationType
    {

        private AmountType[] lineTotalAmountField;

        private AmountType[] chargeTotalAmountField;

        private AmountType[] allowanceTotalAmountField;

        private AmountType[] taxBasisTotalAmountField;

        private AmountType[] taxTotalAmountField;

        private AmountType[] roundingAmountField;

        private AmountType[] grandTotalAmountField;

        private AmountType[] netLineTotalAmountField;

        private AmountType[] includingTaxesLineTotalAmountField;

        [XmlElement("LineTotalAmount")]
        public AmountType[] LineTotalAmount
        {
            get => this.lineTotalAmountField;
            set => this.lineTotalAmountField = value;
        }

        [XmlElement("ChargeTotalAmount")]
        public AmountType[] ChargeTotalAmount
        {
            get => this.chargeTotalAmountField;
            set => this.chargeTotalAmountField = value;
        }

        [XmlElement("AllowanceTotalAmount")]
        public AmountType[] AllowanceTotalAmount
        {
            get => this.allowanceTotalAmountField;
            set => this.allowanceTotalAmountField = value;
        }

        [XmlElement("TaxBasisTotalAmount")]
        public AmountType[] TaxBasisTotalAmount
        {
            get => this.taxBasisTotalAmountField;
            set => this.taxBasisTotalAmountField = value;
        }

        [XmlElement("TaxTotalAmount")]
        public AmountType[] TaxTotalAmount
        {
            get => this.taxTotalAmountField;
            set => this.taxTotalAmountField = value;
        }

        [XmlElement("RoundingAmount")]
        public AmountType[] RoundingAmount
        {
            get => this.roundingAmountField;
            set => this.roundingAmountField = value;
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

        [XmlElement("IncludingTaxesLineTotalAmount")]
        public AmountType[] IncludingTaxesLineTotalAmount
        {
            get => this.includingTaxesLineTotalAmountField;
            set => this.includingTaxesLineTotalAmountField = value;
        }
    }
}