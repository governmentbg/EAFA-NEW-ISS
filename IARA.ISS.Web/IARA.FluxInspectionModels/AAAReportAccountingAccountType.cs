namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportAccountingAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportAccountingAccountType
    {

        private IDType idField;

        private AccountingAccountTypeCodeType typeCodeField;

        private IDType subAccountIDField;

        private TextType nameField;

        private TextType abbreviatedNameField;

        private IDType mainAccountsChartIDField;

        private IDType mainAccountsChartReferenceIDField;

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

        public IDType SubAccountID
        {
            get
            {
                return this.subAccountIDField;
            }
            set
            {
                this.subAccountIDField = value;
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

        public TextType AbbreviatedName
        {
            get
            {
                return this.abbreviatedNameField;
            }
            set
            {
                this.abbreviatedNameField = value;
            }
        }

        public IDType MainAccountsChartID
        {
            get
            {
                return this.mainAccountsChartIDField;
            }
            set
            {
                this.mainAccountsChartIDField = value;
            }
        }

        public IDType MainAccountsChartReferenceID
        {
            get
            {
                return this.mainAccountsChartReferenceIDField;
            }
            set
            {
                this.mainAccountsChartReferenceIDField = value;
            }
        }
    }
}