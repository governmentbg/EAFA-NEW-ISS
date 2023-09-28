namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDHTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDHTradeSettlementMonetarySummationType
    {

        private AmountType lineTotalAmountField;

        private AmountType taxTotalAmountField;

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

        public AmountType TaxTotalAmount
        {
            get
            {
                return this.taxTotalAmountField;
            }
            set
            {
                this.taxTotalAmountField = value;
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