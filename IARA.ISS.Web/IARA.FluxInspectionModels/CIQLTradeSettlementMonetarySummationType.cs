namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQLTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQLTradeSettlementMonetarySummationType
    {

        private AmountType[] grossLineTotalAmountField;

        private AmountType[] netLineTotalAmountField;

        private AmountType[] netIncludingTaxesLineTotalAmountField;

        private AmountType[] includingTaxesLineTotalAmountField;

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

        [XmlElement("IncludingTaxesLineTotalAmount")]
        public AmountType[] IncludingTaxesLineTotalAmount
        {
            get => this.includingTaxesLineTotalAmountField;
            set => this.includingTaxesLineTotalAmountField = value;
        }
    }
}