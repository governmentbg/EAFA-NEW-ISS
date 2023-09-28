namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapProcessedEntity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapProcessedEntityType
    {

        private IDType scenarioIdentificationIDField;

        private NumericType[] scenarioStepNumberNumericField;

        private IDType accountingIdentificationIDField;

        private TextType accountingMethodField;

        private CodeType localAccountingCurrencyCodeField;

        private IDType chartOfAccountsIDField;

        private DateTimeType accountingBookPreparationDateTimeField;

        private DateTimeType accountingBookCreationDateTimeField;

        private ScenarioTypeCodeType scenarioTypeCodeField;

        private TextType commentField;

        private TextType businessProcessLinkageField;

        private AAAWrapOrganizationType[] auditAAAWrapOrganizationField;

        private AAAWrapOrganizationType[] representativeAAAWrapOrganizationField;

        private AAAWrapOrganizationType billedAAAWrapOrganizationField;

        private AAAWrapOrganizationType preparerAAAWrapOrganizationField;

        private AAAWrapOrganizationType originatorAAAWrapOrganizationField;

        private AAAWrapOrganizationType[] recipientAAAWrapOrganizationField;

        private AAAWrapOrganizationType senderAAAWrapOrganizationField;

        private AAAWrapOrganizationType ownerAAAWrapOrganizationField;

        private AAAWrapSoftwareType senderAAAWrapSoftwareField;

        private AAAWrapSoftwareType recipientAAAWrapSoftwareField;

        private AAAWrapSoftwareType[] intermediateAAAWrapSoftwareField;

        private AAAWrapFinancialAccountType[] specifiedAAAWrapFinancialAccountField;

        private AAAWrapJournalListType[] specifiedAAAWrapJournalListField;

        private AAAWrapDayBookType[] specifiedAAAWrapDayBookField;

        private AAAWrapAccountingAccountClassificationType[] specifiedAAAWrapAccountingAccountClassificationField;

        private AAAWrapBundleCollectionType[] specifiedAAAWrapBundleCollectionField;

        private AAAWrapLedgerType[] specifiedAAAWrapLedgerField;

        private AAAWrapTrialBalanceType[] specifiedAAAWrapTrialBalanceField;

        private AAAWrapFormalityType[] specifiedAAAWrapFormalityField;

        public IDType ScenarioIdentificationID
        {
            get
            {
                return this.scenarioIdentificationIDField;
            }
            set
            {
                this.scenarioIdentificationIDField = value;
            }
        }

        [XmlElement("ScenarioStepNumberNumeric")]
        public NumericType[] ScenarioStepNumberNumeric
        {
            get
            {
                return this.scenarioStepNumberNumericField;
            }
            set
            {
                this.scenarioStepNumberNumericField = value;
            }
        }

        public IDType AccountingIdentificationID
        {
            get
            {
                return this.accountingIdentificationIDField;
            }
            set
            {
                this.accountingIdentificationIDField = value;
            }
        }

        public TextType AccountingMethod
        {
            get
            {
                return this.accountingMethodField;
            }
            set
            {
                this.accountingMethodField = value;
            }
        }

        public CodeType LocalAccountingCurrencyCode
        {
            get
            {
                return this.localAccountingCurrencyCodeField;
            }
            set
            {
                this.localAccountingCurrencyCodeField = value;
            }
        }

        public IDType ChartOfAccountsID
        {
            get
            {
                return this.chartOfAccountsIDField;
            }
            set
            {
                this.chartOfAccountsIDField = value;
            }
        }

        public DateTimeType AccountingBookPreparationDateTime
        {
            get
            {
                return this.accountingBookPreparationDateTimeField;
            }
            set
            {
                this.accountingBookPreparationDateTimeField = value;
            }
        }

        public DateTimeType AccountingBookCreationDateTime
        {
            get
            {
                return this.accountingBookCreationDateTimeField;
            }
            set
            {
                this.accountingBookCreationDateTimeField = value;
            }
        }

        public ScenarioTypeCodeType ScenarioTypeCode
        {
            get
            {
                return this.scenarioTypeCodeField;
            }
            set
            {
                this.scenarioTypeCodeField = value;
            }
        }

        public TextType Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        public TextType BusinessProcessLinkage
        {
            get
            {
                return this.businessProcessLinkageField;
            }
            set
            {
                this.businessProcessLinkageField = value;
            }
        }

        [XmlElement("AuditAAAWrapOrganization")]
        public AAAWrapOrganizationType[] AuditAAAWrapOrganization
        {
            get
            {
                return this.auditAAAWrapOrganizationField;
            }
            set
            {
                this.auditAAAWrapOrganizationField = value;
            }
        }

        [XmlElement("RepresentativeAAAWrapOrganization")]
        public AAAWrapOrganizationType[] RepresentativeAAAWrapOrganization
        {
            get
            {
                return this.representativeAAAWrapOrganizationField;
            }
            set
            {
                this.representativeAAAWrapOrganizationField = value;
            }
        }

        public AAAWrapOrganizationType BilledAAAWrapOrganization
        {
            get
            {
                return this.billedAAAWrapOrganizationField;
            }
            set
            {
                this.billedAAAWrapOrganizationField = value;
            }
        }

        public AAAWrapOrganizationType PreparerAAAWrapOrganization
        {
            get
            {
                return this.preparerAAAWrapOrganizationField;
            }
            set
            {
                this.preparerAAAWrapOrganizationField = value;
            }
        }

        public AAAWrapOrganizationType OriginatorAAAWrapOrganization
        {
            get
            {
                return this.originatorAAAWrapOrganizationField;
            }
            set
            {
                this.originatorAAAWrapOrganizationField = value;
            }
        }

        [XmlElement("RecipientAAAWrapOrganization")]
        public AAAWrapOrganizationType[] RecipientAAAWrapOrganization
        {
            get
            {
                return this.recipientAAAWrapOrganizationField;
            }
            set
            {
                this.recipientAAAWrapOrganizationField = value;
            }
        }

        public AAAWrapOrganizationType SenderAAAWrapOrganization
        {
            get
            {
                return this.senderAAAWrapOrganizationField;
            }
            set
            {
                this.senderAAAWrapOrganizationField = value;
            }
        }

        public AAAWrapOrganizationType OwnerAAAWrapOrganization
        {
            get
            {
                return this.ownerAAAWrapOrganizationField;
            }
            set
            {
                this.ownerAAAWrapOrganizationField = value;
            }
        }

        public AAAWrapSoftwareType SenderAAAWrapSoftware
        {
            get
            {
                return this.senderAAAWrapSoftwareField;
            }
            set
            {
                this.senderAAAWrapSoftwareField = value;
            }
        }

        public AAAWrapSoftwareType RecipientAAAWrapSoftware
        {
            get
            {
                return this.recipientAAAWrapSoftwareField;
            }
            set
            {
                this.recipientAAAWrapSoftwareField = value;
            }
        }

        [XmlElement("IntermediateAAAWrapSoftware")]
        public AAAWrapSoftwareType[] IntermediateAAAWrapSoftware
        {
            get
            {
                return this.intermediateAAAWrapSoftwareField;
            }
            set
            {
                this.intermediateAAAWrapSoftwareField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapFinancialAccount")]
        public AAAWrapFinancialAccountType[] SpecifiedAAAWrapFinancialAccount
        {
            get
            {
                return this.specifiedAAAWrapFinancialAccountField;
            }
            set
            {
                this.specifiedAAAWrapFinancialAccountField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapJournalList")]
        public AAAWrapJournalListType[] SpecifiedAAAWrapJournalList
        {
            get
            {
                return this.specifiedAAAWrapJournalListField;
            }
            set
            {
                this.specifiedAAAWrapJournalListField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapDayBook")]
        public AAAWrapDayBookType[] SpecifiedAAAWrapDayBook
        {
            get
            {
                return this.specifiedAAAWrapDayBookField;
            }
            set
            {
                this.specifiedAAAWrapDayBookField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapAccountingAccountClassification")]
        public AAAWrapAccountingAccountClassificationType[] SpecifiedAAAWrapAccountingAccountClassification
        {
            get
            {
                return this.specifiedAAAWrapAccountingAccountClassificationField;
            }
            set
            {
                this.specifiedAAAWrapAccountingAccountClassificationField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapBundleCollection")]
        public AAAWrapBundleCollectionType[] SpecifiedAAAWrapBundleCollection
        {
            get
            {
                return this.specifiedAAAWrapBundleCollectionField;
            }
            set
            {
                this.specifiedAAAWrapBundleCollectionField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapLedger")]
        public AAAWrapLedgerType[] SpecifiedAAAWrapLedger
        {
            get
            {
                return this.specifiedAAAWrapLedgerField;
            }
            set
            {
                this.specifiedAAAWrapLedgerField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapTrialBalance")]
        public AAAWrapTrialBalanceType[] SpecifiedAAAWrapTrialBalance
        {
            get
            {
                return this.specifiedAAAWrapTrialBalanceField;
            }
            set
            {
                this.specifiedAAAWrapTrialBalanceField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapFormality")]
        public AAAWrapFormalityType[] SpecifiedAAAWrapFormality
        {
            get
            {
                return this.specifiedAAAWrapFormalityField;
            }
            set
            {
                this.specifiedAAAWrapFormalityField = value;
            }
        }
    }
}