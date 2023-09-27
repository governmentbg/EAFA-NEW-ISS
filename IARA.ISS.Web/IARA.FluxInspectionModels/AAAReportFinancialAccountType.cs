namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportFinancialAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportFinancialAccountType
    {

        private IDType iBANIDField;

        private FinancialAccountTypeCodeType typeCodeField;

        private TextType accountNameField;

        private CodeType currencyCodeField;

        private AAAReportPartyType servicerAAAReportPartyField;

        private AAAReportPartyType ownerAAAReportPartyField;

        private AAAReportFinancialInstitutionType servicingAAAReportFinancialInstitutionField;

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

        public AAAReportPartyType ServicerAAAReportParty
        {
            get
            {
                return this.servicerAAAReportPartyField;
            }
            set
            {
                this.servicerAAAReportPartyField = value;
            }
        }

        public AAAReportPartyType OwnerAAAReportParty
        {
            get
            {
                return this.ownerAAAReportPartyField;
            }
            set
            {
                this.ownerAAAReportPartyField = value;
            }
        }

        public AAAReportFinancialInstitutionType ServicingAAAReportFinancialInstitution
        {
            get
            {
                return this.servicingAAAReportFinancialInstitutionField;
            }
            set
            {
                this.servicingAAAReportFinancialInstitutionField = value;
            }
        }
    }
}