namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapFinancialAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapFinancialAccountType
    {

        private IDType iBANIDField;

        private FinancialAccountTypeCodeType typeCodeField;

        private TextType accountNameField;

        private CodeType currencyCodeField;

        private AAAWrapPartyType servicerAAAWrapPartyField;

        private AAAWrapPartyType ownerAAAWrapPartyField;

        private AAAWrapFinancialInstitutionType servicingAAAWrapFinancialInstitutionField;

        public IDType IBANID
        {
            get => this.iBANIDField;
            set => this.iBANIDField = value;
        }

        public FinancialAccountTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType AccountName
        {
            get => this.accountNameField;
            set => this.accountNameField = value;
        }

        public CodeType CurrencyCode
        {
            get => this.currencyCodeField;
            set => this.currencyCodeField = value;
        }

        public AAAWrapPartyType ServicerAAAWrapParty
        {
            get => this.servicerAAAWrapPartyField;
            set => this.servicerAAAWrapPartyField = value;
        }

        public AAAWrapPartyType OwnerAAAWrapParty
        {
            get => this.ownerAAAWrapPartyField;
            set => this.ownerAAAWrapPartyField = value;
        }

        public AAAWrapFinancialInstitutionType ServicingAAAWrapFinancialInstitution
        {
            get => this.servicingAAAWrapFinancialInstitutionField;
            set => this.servicingAAAWrapFinancialInstitutionField = value;
        }
    }
}