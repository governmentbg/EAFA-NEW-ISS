namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILBSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILBSupplyChainTradeSettlementType
    {

        private CodeType directionCodeField;

        private CITradeTaxType applicableCITradeTaxField;

        private CIReferencedDocumentType invoiceReferencedCIReferencedDocumentField;

        private CIFinancialAdjustmentType[] specifiedCIFinancialAdjustmentField;

        private CITradeAllowanceChargeType[] specifiedCITradeAllowanceChargeField;

        private CISpecifiedPeriodType billingCISpecifiedPeriodField;

        public CodeType DirectionCode
        {
            get => this.directionCodeField;
            set => this.directionCodeField = value;
        }

        public CITradeTaxType ApplicableCITradeTax
        {
            get => this.applicableCITradeTaxField;
            set => this.applicableCITradeTaxField = value;
        }

        public CIReferencedDocumentType InvoiceReferencedCIReferencedDocument
        {
            get => this.invoiceReferencedCIReferencedDocumentField;
            set => this.invoiceReferencedCIReferencedDocumentField = value;
        }

        [XmlElement("SpecifiedCIFinancialAdjustment")]
        public CIFinancialAdjustmentType[] SpecifiedCIFinancialAdjustment
        {
            get => this.specifiedCIFinancialAdjustmentField;
            set => this.specifiedCIFinancialAdjustmentField = value;
        }

        [XmlElement("SpecifiedCITradeAllowanceCharge")]
        public CITradeAllowanceChargeType[] SpecifiedCITradeAllowanceCharge
        {
            get => this.specifiedCITradeAllowanceChargeField;
            set => this.specifiedCITradeAllowanceChargeField = value;
        }

        public CISpecifiedPeriodType BillingCISpecifiedPeriod
        {
            get => this.billingCISpecifiedPeriodField;
            set => this.billingCISpecifiedPeriodField = value;
        }
    }
}