namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDLTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDLTradeSettlementMonetarySummationType
    {

        private AmountType lineTotalAmountField;

        private AmountType grandTotalAmountField;

        private AmountType[] netLineTotalAmountField;

        private AmountType[] includingTaxesLineTotalAmountField;

        public AmountType LineTotalAmount
        {
            get
            {
                return this.lineTotalAmountField;
            }
            set
            {
                this.lineTotalAmountField = value;
            }
        }

        public AmountType GrandTotalAmount
        {
            get
            {
                return this.grandTotalAmountField;
            }
            set
            {
                this.grandTotalAmountField = value;
            }
        }

        [XmlElement("NetLineTotalAmount")]
        public AmountType[] NetLineTotalAmount
        {
            get
            {
                return this.netLineTotalAmountField;
            }
            set
            {
                this.netLineTotalAmountField = value;
            }
        }

        [XmlElement("IncludingTaxesLineTotalAmount")]
        public AmountType[] IncludingTaxesLineTotalAmount
        {
            get
            {
                return this.includingTaxesLineTotalAmountField;
            }
            set
            {
                this.includingTaxesLineTotalAmountField = value;
            }
        }
    }
}