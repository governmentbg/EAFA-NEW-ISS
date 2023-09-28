namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAATrialBalanceAccountingAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAATrialBalanceAccountingAccountType
    {

        private IDType idField;

        private AccountingAccountTypeCodeType typeCodeField;

        private IDType[] subAccountIDField;

        private TextType nameField;

        private TextType abbreviatedNameField;

        private AccountingAccountStatusCodeType statusCodeField;

        private CodeType groupRankCodeField;

        private DateTimeType openingDateTimeField;

        private TextType openingResponsiblePersonField;

        private IDType aggregationNomenclatureIDField;

        private AccountingDebitCreditStatusCodeType balanceNormalSignCodeField;

        private DateTimeType lockingDateTimeField;

        private TextType lockingResponsiblePersonField;

        private DateTimeType closingDateTimeField;

        private TextType closingResponsiblePersonField;

        private AccountingAccountNatureTypeCodeType natureCodeField;

        private DateTimeType latestDebitPostingDateTimeField;

        private TextType latestDebitPostingResponsiblePersonField;

        private DateTimeType latestCreditPostingDateTimeField;

        private TextType latestCreditPostingResponsiblePersonField;

        private TextType latestMatchingMarkField;

        private TextType latestTickingMarkField;

        private AAATrialBalanceAccountingAccountType[] linkedAAATrialBalanceAccountingAccountField;

        private AAATrialBalanceReportType[] derivedAAATrialBalanceReportField;

        private AAATrialBalanceAccountingAccountClassificationType relatedAAATrialBalanceAccountingAccountClassificationField;

        private AAATrialBalanceAccountingLineMonetaryValueType[] relatedAAATrialBalanceAccountingLineMonetaryValueField;

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

        [XmlElement("SubAccountID")]
        public IDType[] SubAccountID
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

        public IDType AggregationNomenclatureID
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

        [XmlElement("LinkedAAATrialBalanceAccountingAccount")]
        public AAATrialBalanceAccountingAccountType[] LinkedAAATrialBalanceAccountingAccount
        {
            get
            {
                return this.linkedAAATrialBalanceAccountingAccountField;
            }
            set
            {
                this.linkedAAATrialBalanceAccountingAccountField = value;
            }
        }

        [XmlElement("DerivedAAATrialBalanceReport")]
        public AAATrialBalanceReportType[] DerivedAAATrialBalanceReport
        {
            get
            {
                return this.derivedAAATrialBalanceReportField;
            }
            set
            {
                this.derivedAAATrialBalanceReportField = value;
            }
        }

        public AAATrialBalanceAccountingAccountClassificationType RelatedAAATrialBalanceAccountingAccountClassification
        {
            get
            {
                return this.relatedAAATrialBalanceAccountingAccountClassificationField;
            }
            set
            {
                this.relatedAAATrialBalanceAccountingAccountClassificationField = value;
            }
        }

        [XmlElement("RelatedAAATrialBalanceAccountingLineMonetaryValue")]
        public AAATrialBalanceAccountingLineMonetaryValueType[] RelatedAAATrialBalanceAccountingLineMonetaryValue
        {
            get
            {
                return this.relatedAAATrialBalanceAccountingLineMonetaryValueField;
            }
            set
            {
                this.relatedAAATrialBalanceAccountingLineMonetaryValueField = value;
            }
        }
    }
}