namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQHTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQHTradeSettlementMonetarySummationType
    {

        private AmountType[] allowanceTotalAmountField;

        private AmountType[] chargeTotalAmountField;

        private AmountType[] grossLineTotalAmountField;

        private AmountType[] netLineTotalAmountField;

        private AmountType[] netIncludingTaxesLineTotalAmountField;

        private AmountType[] totalAllowanceChargeAmountField;

        private AmountType[] taxTotalAmountField;

        private AmountType[] grandTotalAmountField;

        private AmountType[] includingTaxesLineTotalAmountField;

        [XmlElement("AllowanceTotalAmount")]
        public AmountType[] AllowanceTotalAmount
        {
            get => this.allowanceTotalAmountField;
            set => this.allowanceTotalAmountField = value;
        }

        [XmlElement("ChargeTotalAmount")]
        public AmountType[] ChargeTotalAmount
        {
            get => this.chargeTotalAmountField;
            set => this.chargeTotalAmountField = value;
        }

        [XmlElement("GrossLineTotalAmount")]
        public AmountType[] GrossLineTotalAmount
        {
            get => this.grossLineTotalAmountField;
            set => this.grossLineTotalAmountField = value;
        }

        [XmlElement("NetLineTotalAmount")]
        public AmountType[] NetLineTotalAmount
        {
            get => this.netLineTotalAmountField;
            set => this.netLineTotalAmountField = value;
        }

        [XmlElement("NetIncludingTaxesLineTotalAmount")]
        public AmountType[] NetIncludingTaxesLineTotalAmount
        {
            get => this.netIncludingTaxesLineTotalAmountField;
            set => this.netIncludingTaxesLineTotalAmountField = value;
        }

        [XmlElement("TotalAllowanceChargeAmount")]
        public AmountType[] TotalAllowanceChargeAmount
        {
            get => this.totalAllowanceChargeAmountField;
            set => this.totalAllowanceChargeAmountField = value;
        }

        [XmlElement("TaxTotalAmount")]
        public AmountType[] TaxTotalAmount
        {
            get => this.taxTotalAmountField;
            set => this.taxTotalAmountField = value;
        }

        [XmlElement("GrandTotalAmount")]
        public AmountType[] GrandTotalAmount
        {
            get => this.grandTotalAmountField;
            set => this.grandTotalAmountField = value;
        }

        [XmlElement("IncludingTaxesLineTotalAmount")]
        public AmountType[] IncludingTaxesLineTotalAmount
        {
            get => this.includingTaxesLineTotalAmountField;
            set => this.includingTaxesLineTotalAmountField = value;
        }
    }
}