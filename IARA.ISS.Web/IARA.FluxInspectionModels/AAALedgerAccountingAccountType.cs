namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAALedgerAccountingAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAALedgerAccountingAccountType
    {

        private IDType idField;

        private AccountingAccountTypeCodeType typeCodeField;

        private AccountingDocumentCodeType setTriggerCodeField;

        private AccountingAmountTypeCodeType amountTypeCodeField;

        private IDType subAccountIDField;

        private TextType nameField;

        private TextType abbreviatedNameField;

        private IDType mainAccountsChartIDField;

        private IDType mainAccountsChartReferenceIDField;

        private AAALedgerReportType[] derivedAAALedgerReportField;

        private AAALedgerAccountingEntryLineType[] includedAAALedgerAccountingEntryLineField;

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

        [XmlElement("DerivedAAALedgerReport")]
        public AAALedgerReportType[] DerivedAAALedgerReport
        {
            get
            {
                return this.derivedAAALedgerReportField;
            }
            set
            {
                this.derivedAAALedgerReportField = value;
            }
        }

        [XmlElement("IncludedAAALedgerAccountingEntryLine")]
        public AAALedgerAccountingEntryLineType[] IncludedAAALedgerAccountingEntryLine
        {
            get
            {
                return this.includedAAALedgerAccountingEntryLineField;
            }
            set
            {
                this.includedAAALedgerAccountingEntryLineField = value;
            }
        }
    }
}