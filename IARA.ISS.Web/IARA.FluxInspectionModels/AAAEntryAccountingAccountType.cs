namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAEntryAccountingAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAEntryAccountingAccountType
    {

        private IDType idField;

        private IDType subAccountIDField;

        private TextType nameField;

        private TextType abbreviatedNameField;

        private AccountingDocumentCodeType setTriggerCodeField;

        private AccountingAccountTypeCodeType typeCodeField;

        private AccountingAmountTypeCodeType amountTypeCodeField;

        private IDType mainAccountsChartIDField;

        private IDType mainAccountsChartReferenceIDField;

        private DateTimeType accountingYearEndDateTimeField;

        private AAAEntryReportType[] derivedAAAEntryReportField;

        private AAAEntryCapitalAssetType fiscalAAAEntryCapitalAssetField;

        private AAAEntryCapitalAssetType economicAAAEntryCapitalAssetField;

        private AAAEntryCapitalAssetType iFRSAAAEntryCapitalAssetField;

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

        public DateTimeType AccountingYearEndDateTime
        {
            get
            {
                return this.accountingYearEndDateTimeField;
            }
            set
            {
                this.accountingYearEndDateTimeField = value;
            }
        }

        [XmlElement("DerivedAAAEntryReport")]
        public AAAEntryReportType[] DerivedAAAEntryReport
        {
            get
            {
                return this.derivedAAAEntryReportField;
            }
            set
            {
                this.derivedAAAEntryReportField = value;
            }
        }

        public AAAEntryCapitalAssetType FiscalAAAEntryCapitalAsset
        {
            get
            {
                return this.fiscalAAAEntryCapitalAssetField;
            }
            set
            {
                this.fiscalAAAEntryCapitalAssetField = value;
            }
        }

        public AAAEntryCapitalAssetType EconomicAAAEntryCapitalAsset
        {
            get
            {
                return this.economicAAAEntryCapitalAssetField;
            }
            set
            {
                this.economicAAAEntryCapitalAssetField = value;
            }
        }

        public AAAEntryCapitalAssetType IFRSAAAEntryCapitalAsset
        {
            get
            {
                return this.iFRSAAAEntryCapitalAssetField;
            }
            set
            {
                this.iFRSAAAEntryCapitalAssetField = value;
            }
        }
    }
}