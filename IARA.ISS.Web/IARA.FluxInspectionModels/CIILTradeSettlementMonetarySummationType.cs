namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILTradeSettlementMonetarySummationType
    {

        private AmountType[] lineTotalAmountField;

        private AmountType[] chargeTotalAmountField;

        private AmountType[] allowanceTotalAmountField;

        private AmountType[] taxBasisTotalAmountField;

        private AmountType[] taxTotalAmountField;

        private AmountType[] informationAmountField;

        private AmountType[] totalAllowanceChargeAmountField;

        private AmountType[] productWeightLossInformationAmountField;

        private AmountType[] totalRetailValueInformationAmountField;

        private AmountType[] grossLineTotalAmountField;

        private AmountType[] netLineTotalAmountField;

        private AmountType[] netIncludingTaxesLineTotalAmountField;

        private AmountType grandTotalAmountField;

        private AmountType includingTaxesLineTotalAmountField;

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

        [XmlElement("InformationAmount")]
        public AmountType[] InformationAmount
        {
            get => this.informationAmountField;
            set => this.informationAmountField = value;
        }

        [XmlElement("TotalAllowanceChargeAmount")]
        public AmountType[] TotalAllowanceChargeAmount
        {
            get => this.totalAllowanceChargeAmountField;
            set => this.totalAllowanceChargeAmountField = value;
        }

        [XmlElement("ProductWeightLossInformationAmount")]
        public AmountType[] ProductWeightLossInformationAmount
        {
            get => this.productWeightLossInformationAmountField;
            set => this.productWeightLossInformationAmountField = value;
        }

        [XmlElement("TotalRetailValueInformationAmount")]
        public AmountType[] TotalRetailValueInformationAmount
        {
            get => this.totalRetailValueInformationAmountField;
            set => this.totalRetailValueInformationAmountField = value;
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

        public AmountType GrandTotalAmount
        {
            get => this.grandTotalAmountField;
            set => this.grandTotalAmountField = value;
        }

        public AmountType IncludingTaxesLineTotalAmount
        {
            get => this.includingTaxesLineTotalAmountField;
            set => this.includingTaxesLineTotalAmountField = value;
        }
    }
}