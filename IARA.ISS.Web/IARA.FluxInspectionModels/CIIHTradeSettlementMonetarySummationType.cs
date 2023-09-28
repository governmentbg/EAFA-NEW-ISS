namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIIHTradeSettlementMonetarySummation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIIHTradeSettlementMonetarySummationType
    {

        private AmountType[] lineTotalAmountField;

        private AmountType[] chargeTotalAmountField;

        private AmountType[] allowanceTotalAmountField;

        private AmountType[] taxBasisTotalAmountField;

        private AmountType[] taxTotalAmountField;

        private AmountType[] roundingAmountField;

        private AmountType[] grandTotalAmountField;

        private AmountType[] informationAmountField;

        private AmountType[] totalPrepaidAmountField;

        private AmountType[] totalDiscountAmountField;

        private AmountType[] totalAllowanceChargeAmountField;

        private AmountType[] duePayableAmountField;

        private AmountType[] retailValueExcludingTaxInformationAmountField;

        private AmountType[] totalDepositFeeInformationAmountField;

        private AmountType[] productValueExcludingTobaccoTaxInformationAmountField;

        private AmountType[] totalRetailValueInformationAmountField;

        private AmountType[] grossLineTotalAmountField;

        private AmountType[] netLineTotalAmountField;

        private AmountType[] netIncludingTaxesLineTotalAmountField;

        private AmountType insuranceChargeTotalAmountField;

        private AmountType[] includingTaxesLineTotalAmountField;

        private CIReferencedDocumentType[] specifiedCIReferencedDocumentField;

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

        [XmlElement("InformationAmount")]
        public AmountType[] InformationAmount
        {
            get => this.informationAmountField;
            set => this.informationAmountField = value;
        }

        [XmlElement("TotalPrepaidAmount")]
        public AmountType[] TotalPrepaidAmount
        {
            get => this.totalPrepaidAmountField;
            set => this.totalPrepaidAmountField = value;
        }

        [XmlElement("TotalDiscountAmount")]
        public AmountType[] TotalDiscountAmount
        {
            get => this.totalDiscountAmountField;
            set => this.totalDiscountAmountField = value;
        }

        [XmlElement("TotalAllowanceChargeAmount")]
        public AmountType[] TotalAllowanceChargeAmount
        {
            get => this.totalAllowanceChargeAmountField;
            set => this.totalAllowanceChargeAmountField = value;
        }

        [XmlElement("DuePayableAmount")]
        public AmountType[] DuePayableAmount
        {
            get => this.duePayableAmountField;
            set => this.duePayableAmountField = value;
        }

        [XmlElement("RetailValueExcludingTaxInformationAmount")]
        public AmountType[] RetailValueExcludingTaxInformationAmount
        {
            get => this.retailValueExcludingTaxInformationAmountField;
            set => this.retailValueExcludingTaxInformationAmountField = value;
        }

        [XmlElement("TotalDepositFeeInformationAmount")]
        public AmountType[] TotalDepositFeeInformationAmount
        {
            get => this.totalDepositFeeInformationAmountField;
            set => this.totalDepositFeeInformationAmountField = value;
        }

        [XmlElement("ProductValueExcludingTobaccoTaxInformationAmount")]
        public AmountType[] ProductValueExcludingTobaccoTaxInformationAmount
        {
            get => this.productValueExcludingTobaccoTaxInformationAmountField;
            set => this.productValueExcludingTobaccoTaxInformationAmountField = value;
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

        public AmountType InsuranceChargeTotalAmount
        {
            get => this.insuranceChargeTotalAmountField;
            set => this.insuranceChargeTotalAmountField = value;
        }

        [XmlElement("IncludingTaxesLineTotalAmount")]
        public AmountType[] IncludingTaxesLineTotalAmount
        {
            get => this.includingTaxesLineTotalAmountField;
            set => this.includingTaxesLineTotalAmountField = value;
        }

        [XmlElement("SpecifiedCIReferencedDocument")]
        public CIReferencedDocumentType[] SpecifiedCIReferencedDocument
        {
            get => this.specifiedCIReferencedDocumentField;
            set => this.specifiedCIReferencedDocumentField = value;
        }
    }
}