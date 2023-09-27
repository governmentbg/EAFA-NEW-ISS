namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAALedgerFinancialAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAALedgerFinancialAccountType
    {

        private IDType iBANIDField;

        private FinancialAccountTypeCodeType typeCodeField;

        private TextType accountNameField;

        public IDType IBANID
        {
            get
            {
                return this.iBANIDField;
            }
            set
            {
                this.iBANIDField = value;
            }
        }

        public FinancialAccountTypeCodeType TypeCode
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

        public TextType AccountName
        {
            get
            {
                return this.accountNameField;
            }
            set
            {
                this.accountNameField = value;
            }
        }
    }
}