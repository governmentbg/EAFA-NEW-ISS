namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeAccountingAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeAccountingAccountType
    {

        private IDType idField;

        private AccountingDocumentCodeType setTriggerCodeField;

        private AccountingAccountTypeCodeType typeCodeField;

        private AccountingAmountTypeCodeType amountTypeCodeField;

        private TextType nameField;

        private TextType costReferenceDimensionPatternField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public AccountingDocumentCodeType SetTriggerCode
        {
            get
            {
                return this.setTriggerCodeField;
            }
            set
            {
                this.setTriggerCodeField = value;
            }
        }

        public AccountingAccountTypeCodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public AccountingAmountTypeCodeType AmountTypeCode
        {
            get
            {
                return this.amountTypeCodeField;
            }
            set
            {
                this.amountTypeCodeField = value;
            }
        }

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public TextType CostReferenceDimensionPattern
        {
            get
            {
                return this.costReferenceDimensionPatternField;
            }
            set
            {
                this.costReferenceDimensionPatternField = value;
            }
        }
    }
}