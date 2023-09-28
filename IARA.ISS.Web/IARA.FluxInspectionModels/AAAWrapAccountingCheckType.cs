namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapAccountingCheck", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapAccountingCheckType
    {

        private AccountingDocumentTypeCodeType documentTypeCodeField;

        private QuantityType totalElementQuantityField;

        private AmountType totalDebitAmountField;

        private AmountType totalCreditAmountField;

        private AAAWrapAccountingAccountBoundaryType[] excludedAAAWrapAccountingAccountBoundaryField;

        private AAAWrapAccountingAccountBoundaryType[] includedAAAWrapAccountingAccountBoundaryField;

        public AccountingDocumentTypeCodeType DocumentTypeCode
        {
            get
            {
                return this.documentTypeCodeField;
            }
            set
            {
                this.documentTypeCodeField = value;
            }
        }

        public QuantityType TotalElementQuantity
        {
            get
            {
                return this.totalElementQuantityField;
            }
            set
            {
                this.totalElementQuantityField = value;
            }
        }

        public AmountType TotalDebitAmount
        {
            get
            {
                return this.totalDebitAmountField;
            }
            set
            {
                this.totalDebitAmountField = value;
            }
        }

        public AmountType TotalCreditAmount
        {
            get
            {
                return this.totalCreditAmountField;
            }
            set
            {
                this.totalCreditAmountField = value;
            }
        }

        [XmlElement("ExcludedAAAWrapAccountingAccountBoundary")]
        public AAAWrapAccountingAccountBoundaryType[] ExcludedAAAWrapAccountingAccountBoundary
        {
            get
            {
                return this.excludedAAAWrapAccountingAccountBoundaryField;
            }
            set
            {
                this.excludedAAAWrapAccountingAccountBoundaryField = value;
            }
        }

        [XmlElement("IncludedAAAWrapAccountingAccountBoundary")]
        public AAAWrapAccountingAccountBoundaryType[] IncludedAAAWrapAccountingAccountBoundary
        {
            get
            {
                return this.includedAAAWrapAccountingAccountBoundaryField;
            }
            set
            {
                this.includedAAAWrapAccountingAccountBoundaryField = value;
            }
        }
    }
}