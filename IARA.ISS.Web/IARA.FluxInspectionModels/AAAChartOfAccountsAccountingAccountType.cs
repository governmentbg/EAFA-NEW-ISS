namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAChartOfAccountsAccountingAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAChartOfAccountsAccountingAccountType
    {

        private IDType idField;

        private AccountingAccountTypeCodeType typeCodeField;

        private IDType subAccountIDField;

        private TextType nameField;

        private TextType abbreviatedNameField;

        private AccountingAccountStatusCodeType statusCodeField;

        private CodeType groupRankCodeField;

        private DateTimeType openingDateTimeField;

        private TextType openingResponsiblePersonField;

        private IDType[] aggregationNomenclatureIDField;

        private AccountingDebitCreditStatusCodeType balanceNormalSignCodeField;

        private AccountingAccountBalanceReopeningTypeCodeType balanceReopeningModeCodeField;

        private DateTimeType lockingDateTimeField;

        private TextType lockingResponsiblePersonField;

        private DateTimeType closingDateTimeField;

        private TextType closingResponsiblePersonField;

        private AccountingAccountNatureTypeCodeType natureCodeField;

        private IndicatorType mandatorySecondaryAccountingIndicatorField;

        private DateTimeType latestDebitPostingDateTimeField;

        private TextType latestDebitPostingResponsiblePersonField;

        private DateTimeType latestCreditPostingDateTimeField;

        private TextType latestCreditPostingResponsiblePersonField;

        private IndicatorType matchingMarkIndicatorField;

        private TextType latestMatchingMarkField;

        private IndicatorType tickingMarkIndicatorField;

        private TextType latestTickingMarkField;

        private CodeType languageCodeField;

        private CodeType currencyCodeField;

        private IDType electronicInvoiceURIIDField;

        private AAAChartOfAccountsPersonType[] updateAuthorizedAAAChartOfAccountsPersonField;

        private AAAChartOfAccountsAccountingAccountClassificationType relatedAAAChartOfAccountsAccountingAccountClassificationField;

        private AAAChartOfAccountsAccountingAccountBoundaryType[] inclusiveAAAChartOfAccountsAccountingAccountBoundaryField;

        private AAAChartOfAccountsAccountingAccountPatternType mainAAAChartOfAccountsAccountingAccountPatternField;

        private AAAChartOfAccountsAccountingAccountPatternType subordinateAAAChartOfAccountsAccountingAccountPatternField;

        private AAAChartOfAccountsTaxType[] specifiedAAAChartOfAccountsTaxField;

        private AAAChartOfAccountsFinancialAccountType[] specifiedAAAChartOfAccountsFinancialAccountField;

        private AAAChartOfAccountsPaymentInstructionType specifiedAAAChartOfAccountsPaymentInstructionField;

        private AAAChartOfAccountsAccountingAccountCreditRiskType relatedAAAChartOfAccountsAccountingAccountCreditRiskField;

        private AAAChartOfAccountsAccountingAccountType[] linkedAAAChartOfAccountsAccountingAccountField;

        private AAAChartOfAccountsReportType[] derivedAAAChartOfAccountsReportField;

        private AAAChartOfAccountsOrganizationType relatedAAAChartOfAccountsOrganizationField;

        private AAAChartOfAccountsAccountingAccountBoundaryType[] exclusiveAAAChartOfAccountsAccountingAccountBoundaryField;

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

        public AccountingAccountStatusCodeType StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        public CodeType GroupRankCode
        {
            get
            {
                return this.groupRankCodeField;
            }
            set
            {
                this.groupRankCodeField = value;
            }
        }

        public DateTimeType OpeningDateTime
        {
            get
            {
                return this.openingDateTimeField;
            }
            set
            {
                this.openingDateTimeField = value;
            }
        }

        public TextType OpeningResponsiblePerson
        {
            get
            {
                return this.openingResponsiblePersonField;
            }
            set
            {
                this.openingResponsiblePersonField = value;
            }
        }

        [XmlElement("AggregationNomenclatureID")]
        public IDType[] AggregationNomenclatureID
        {
            get
            {
                return this.aggregationNomenclatureIDField;
            }
            set
            {
                this.aggregationNomenclatureIDField = value;
            }
        }

        public AccountingDebitCreditStatusCodeType BalanceNormalSignCode
        {
            get
            {
                return this.balanceNormalSignCodeField;
            }
            set
            {
                this.balanceNormalSignCodeField = value;
            }
        }

        public AccountingAccountBalanceReopeningTypeCodeType BalanceReopeningModeCode
        {
            get
            {
                return this.balanceReopeningModeCodeField;
            }
            set
            {
                this.balanceReopeningModeCodeField = value;
            }
        }

        public DateTimeType LockingDateTime
        {
            get
            {
                return this.lockingDateTimeField;
            }
            set
            {
                this.lockingDateTimeField = value;
            }
        }

        public TextType LockingResponsiblePerson
        {
            get
            {
                return this.lockingResponsiblePersonField;
            }
            set
            {
                this.lockingResponsiblePersonField = value;
            }
        }

        public DateTimeType ClosingDateTime
        {
            get
            {
                return this.closingDateTimeField;
            }
            set
            {
                this.closingDateTimeField = value;
            }
        }

        public TextType ClosingResponsiblePerson
        {
            get
            {
                return this.closingResponsiblePersonField;
            }
            set
            {
                this.closingResponsiblePersonField = value;
            }
        }

        public AccountingAccountNatureTypeCodeType NatureCode
        {
            get
            {
                return this.natureCodeField;
            }
            set
            {
                this.natureCodeField = value;
            }
        }

        public IndicatorType MandatorySecondaryAccountingIndicator
        {
            get
            {
                return this.mandatorySecondaryAccountingIndicatorField;
            }
            set
            {
                this.mandatorySecondaryAccountingIndicatorField = value;
            }
        }

        public DateTimeType LatestDebitPostingDateTime
        {
            get
            {
                return this.latestDebitPostingDateTimeField;
            }
            set
            {
                this.latestDebitPostingDateTimeField = value;
            }
        }

        public TextType LatestDebitPostingResponsiblePerson
        {
            get
            {
                return this.latestDebitPostingResponsiblePersonField;
            }
            set
            {
                this.latestDebitPostingResponsiblePersonField = value;
            }
        }

        public DateTimeType LatestCreditPostingDateTime
        {
            get
            {
                return this.latestCreditPostingDateTimeField;
            }
            set
            {
                this.latestCreditPostingDateTimeField = value;
            }
        }

        public TextType LatestCreditPostingResponsiblePerson
        {
            get
            {
                return this.latestCreditPostingResponsiblePersonField;
            }
            set
            {
                this.latestCreditPostingResponsiblePersonField = value;
            }
        }

        public IndicatorType MatchingMarkIndicator
        {
            get
            {
                return this.matchingMarkIndicatorField;
            }
            set
            {
                this.matchingMarkIndicatorField = value;
            }
        }

        public TextType LatestMatchingMark
        {
            get
            {
                return this.latestMatchingMarkField;
            }
            set
            {
                this.latestMatchingMarkField = value;
            }
        }

        public IndicatorType TickingMarkIndicator
        {
            get
            {
                return this.tickingMarkIndicatorField;
            }
            set
            {
                this.tickingMarkIndicatorField = value;
            }
        }

        public TextType LatestTickingMark
        {
            get
            {
                return this.latestTickingMarkField;
            }
            set
            {
                this.latestTickingMarkField = value;
            }
        }

        public CodeType LanguageCode
        {
            get
            {
                return this.languageCodeField;
            }
            set
            {
                this.languageCodeField = value;
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

        public IDType ElectronicInvoiceURIID
        {
            get
            {
                return this.electronicInvoiceURIIDField;
            }
            set
            {
                this.electronicInvoiceURIIDField = value;
            }
        }

        [XmlElement("UpdateAuthorizedAAAChartOfAccountsPerson")]
        public AAAChartOfAccountsPersonType[] UpdateAuthorizedAAAChartOfAccountsPerson
        {
            get
            {
                return this.updateAuthorizedAAAChartOfAccountsPersonField;
            }
            set
            {
                this.updateAuthorizedAAAChartOfAccountsPersonField = value;
            }
        }

        public AAAChartOfAccountsAccountingAccountClassificationType RelatedAAAChartOfAccountsAccountingAccountClassification
        {
            get
            {
                return this.relatedAAAChartOfAccountsAccountingAccountClassificationField;
            }
            set
            {
                this.relatedAAAChartOfAccountsAccountingAccountClassificationField = value;
            }
        }

        [XmlElement("InclusiveAAAChartOfAccountsAccountingAccountBoundary")]
        public AAAChartOfAccountsAccountingAccountBoundaryType[] InclusiveAAAChartOfAccountsAccountingAccountBoundary
        {
            get
            {
                return this.inclusiveAAAChartOfAccountsAccountingAccountBoundaryField;
            }
            set
            {
                this.inclusiveAAAChartOfAccountsAccountingAccountBoundaryField = value;
            }
        }

        public AAAChartOfAccountsAccountingAccountPatternType MainAAAChartOfAccountsAccountingAccountPattern
        {
            get
            {
                return this.mainAAAChartOfAccountsAccountingAccountPatternField;
            }
            set
            {
                this.mainAAAChartOfAccountsAccountingAccountPatternField = value;
            }
        }

        public AAAChartOfAccountsAccountingAccountPatternType SubordinateAAAChartOfAccountsAccountingAccountPattern
        {
            get
            {
                return this.subordinateAAAChartOfAccountsAccountingAccountPatternField;
            }
            set
            {
                this.subordinateAAAChartOfAccountsAccountingAccountPatternField = value;
            }
        }

        [XmlElement("SpecifiedAAAChartOfAccountsTax")]
        public AAAChartOfAccountsTaxType[] SpecifiedAAAChartOfAccountsTax
        {
            get
            {
                return this.specifiedAAAChartOfAccountsTaxField;
            }
            set
            {
                this.specifiedAAAChartOfAccountsTaxField = value;
            }
        }

        [XmlElement("SpecifiedAAAChartOfAccountsFinancialAccount")]
        public AAAChartOfAccountsFinancialAccountType[] SpecifiedAAAChartOfAccountsFinancialAccount
        {
            get
            {
                return this.specifiedAAAChartOfAccountsFinancialAccountField;
            }
            set
            {
                this.specifiedAAAChartOfAccountsFinancialAccountField = value;
            }
        }

        public AAAChartOfAccountsPaymentInstructionType SpecifiedAAAChartOfAccountsPaymentInstruction
        {
            get
            {
                return this.specifiedAAAChartOfAccountsPaymentInstructionField;
            }
            set
            {
                this.specifiedAAAChartOfAccountsPaymentInstructionField = value;
            }
        }

        public AAAChartOfAccountsAccountingAccountCreditRiskType RelatedAAAChartOfAccountsAccountingAccountCreditRisk
        {
            get
            {
                return this.relatedAAAChartOfAccountsAccountingAccountCreditRiskField;
            }
            set
            {
                this.relatedAAAChartOfAccountsAccountingAccountCreditRiskField = value;
            }
        }

        [XmlElement("LinkedAAAChartOfAccountsAccountingAccount")]
        public AAAChartOfAccountsAccountingAccountType[] LinkedAAAChartOfAccountsAccountingAccount
        {
            get
            {
                return this.linkedAAAChartOfAccountsAccountingAccountField;
            }
            set
            {
                this.linkedAAAChartOfAccountsAccountingAccountField = value;
            }
        }

        [XmlElement("DerivedAAAChartOfAccountsReport")]
        public AAAChartOfAccountsReportType[] DerivedAAAChartOfAccountsReport
        {
            get
            {
                return this.derivedAAAChartOfAccountsReportField;
            }
            set
            {
                this.derivedAAAChartOfAccountsReportField = value;
            }
        }

        public AAAChartOfAccountsOrganizationType RelatedAAAChartOfAccountsOrganization
        {
            get
            {
                return this.relatedAAAChartOfAccountsOrganizationField;
            }
            set
            {
                this.relatedAAAChartOfAccountsOrganizationField = value;
            }
        }

        [XmlElement("ExclusiveAAAChartOfAccountsAccountingAccountBoundary")]
        public AAAChartOfAccountsAccountingAccountBoundaryType[] ExclusiveAAAChartOfAccountsAccountingAccountBoundary
        {
            get
            {
                return this.exclusiveAAAChartOfAccountsAccountingAccountBoundaryField;
            }
            set
            {
                this.exclusiveAAAChartOfAccountsAccountingAccountBoundaryField = value;
            }
        }
    }
}