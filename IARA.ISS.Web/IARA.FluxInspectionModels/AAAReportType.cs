using IARA.FluxInspectionModels;

namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReport", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportType
    {

        private IDType[] nextInformationIDField;

        private TextType nameField;

        private IDType itemIDField;

        private AAAReportType[] dependentAAAReportField;

        private AAAReportFinancialAccountType specifiedAAAReportFinancialAccountField;

        private AAAReportExpectedInformationType[] specifiedAAAReportExpectedInformationField;

        private AAAReportPersonType specifiedAAAReportPersonField;

        private AAAAddressType specifiedAAAAddressField;

        private AAAReportSoftwareType specifiedAAAReportSoftwareField;

        private AAAReportOrganizationType specifiedAAAReportOrganizationField;

        private AAAReportAccountingAccountType specifiedAAAReportAccountingAccountField;

        private AAAReportDocumentType includedAAAReportDocumentField;

        private AAAReportAccountingPeriodType[] specifiedAAAReportAccountingPeriodField;

        private AAAReportPaymentTermsType specifiedAAAReportPaymentTermsField;

        [XmlElement("NextInformationID")]
        public IDType[] NextInformationID
        {
            get
            {
                return this.nextInformationIDField;
            }
            set
            {
                this.nextInformationIDField = value;
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

        public IDType ItemID
        {
            get
            {
                return this.itemIDField;
            }
            set
            {
                this.itemIDField = value;
            }
        }

        [XmlElement("DependentAAAReport")]
        public AAAReportType[] DependentAAAReport
        {
            get
            {
                return this.dependentAAAReportField;
            }
            set
            {
                this.dependentAAAReportField = value;
            }
        }

        public AAAReportFinancialAccountType SpecifiedAAAReportFinancialAccount
        {
            get
            {
                return this.specifiedAAAReportFinancialAccountField;
            }
            set
            {
                this.specifiedAAAReportFinancialAccountField = value;
            }
        }

        [XmlElement("SpecifiedAAAReportExpectedInformation")]
        public AAAReportExpectedInformationType[] SpecifiedAAAReportExpectedInformation
        {
            get
            {
                return this.specifiedAAAReportExpectedInformationField;
            }
            set
            {
                this.specifiedAAAReportExpectedInformationField = value;
            }
        }

        public AAAReportPersonType SpecifiedAAAReportPerson
        {
            get
            {
                return this.specifiedAAAReportPersonField;
            }
            set
            {
                this.specifiedAAAReportPersonField = value;
            }
        }

        public AAAAddressType SpecifiedAAAAddress
        {
            get
            {
                return this.specifiedAAAAddressField;
            }
            set
            {
                this.specifiedAAAAddressField = value;
            }
        }

        public AAAReportSoftwareType SpecifiedAAAReportSoftware
        {
            get
            {
                return this.specifiedAAAReportSoftwareField;
            }
            set
            {
                this.specifiedAAAReportSoftwareField = value;
            }
        }

        public AAAReportOrganizationType SpecifiedAAAReportOrganization
        {
            get
            {
                return this.specifiedAAAReportOrganizationField;
            }
            set
            {
                this.specifiedAAAReportOrganizationField = value;
            }
        }

        public AAAReportAccountingAccountType SpecifiedAAAReportAccountingAccount
        {
            get
            {
                return this.specifiedAAAReportAccountingAccountField;
            }
            set
            {
                this.specifiedAAAReportAccountingAccountField = value;
            }
        }

        public AAAReportDocumentType IncludedAAAReportDocument
        {
            get
            {
                return this.includedAAAReportDocumentField;
            }
            set
            {
                this.includedAAAReportDocumentField = value;
            }
        }

        [XmlElement("SpecifiedAAAReportAccountingPeriod")]
        public AAAReportAccountingPeriodType[] SpecifiedAAAReportAccountingPeriod
        {
            get
            {
                return this.specifiedAAAReportAccountingPeriodField;
            }
            set
            {
                this.specifiedAAAReportAccountingPeriodField = value;
            }
        }

        public AAAReportPaymentTermsType SpecifiedAAAReportPaymentTerms
        {
            get
            {
                return this.specifiedAAAReportPaymentTermsField;
            }
            set
            {
                this.specifiedAAAReportPaymentTermsField = value;
            }
        }
    }
}