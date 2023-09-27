namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAChartOfAccountsFinancialAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAChartOfAccountsFinancialAccountType
    {

        private IDType iBANIDField;

        private FinancialAccountTypeCodeType typeCodeField;

        private TextType accountNameField;

        private CodeType currencyCodeField;

        private AAAChartOfAccountsPartyType servicerAAAChartOfAccountsPartyField;

        private AAAChartOfAccountsPartyType ownerAAAChartOfAccountsPartyField;

        private AAAChartOfAccountsFinancialInstitutionType servicingAAAChartOfAccountsFinancialInstitutionField;

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

        public CodeType CurrencyCode
        {
            get
            {
                return this.currencyCodeField;
            }
            set
            {
                this.currencyCodeField = value;
            }
        }

        public AAAChartOfAccountsPartyType ServicerAAAChartOfAccountsParty
        {
            get
            {
                return this.servicerAAAChartOfAccountsPartyField;
            }
            set
            {
                this.servicerAAAChartOfAccountsPartyField = value;
            }
        }

        public AAAChartOfAccountsPartyType OwnerAAAChartOfAccountsParty
        {
            get
            {
                return this.ownerAAAChartOfAccountsPartyField;
            }
            set
            {
                this.ownerAAAChartOfAccountsPartyField = value;
            }
        }

        public AAAChartOfAccountsFinancialInstitutionType ServicingAAAChartOfAccountsFinancialInstitution
        {
            get
            {
                return this.servicingAAAChartOfAccountsFinancialInstitutionField;
            }
            set
            {
                this.servicingAAAChartOfAccountsFinancialInstitutionField = value;
            }
        }
    }
}