namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWAuthorization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWAuthorizationType
    {

        private IndicatorType grantedIndicatorField;

        private TextType conditionField;

        private TextType objectionField;

        private DateTimeType decisionDateTimeField;

        private IDType subjectIDField;

        private TMWOrganizationType authorityTMWOrganizationField;

        private TMWDelimitedPeriodType validityTMWDelimitedPeriodField;

        public IndicatorType GrantedIndicator
        {
            get => this.grantedIndicatorField;
            set => this.grantedIndicatorField = value;
        }

        public TextType Condition
        {
            get => this.conditionField;
            set => this.conditionField = value;
        }

        public TextType Objection
        {
            get => this.objectionField;
            set => this.objectionField = value;
        }

        public DateTimeType DecisionDateTime
        {
            get => this.decisionDateTimeField;
            set => this.decisionDateTimeField = value;
        }

        public IDType SubjectID
        {
            get => this.subjectIDField;
            set => this.subjectIDField = value;
        }

        public TMWOrganizationType AuthorityTMWOrganization
        {
            get => this.authorityTMWOrganizationField;
            set => this.authorityTMWOrganizationField = value;
        }

        public TMWDelimitedPeriodType ValidityTMWDelimitedPeriod
        {
            get => this.validityTMWDelimitedPeriodField;
            set => this.validityTMWDelimitedPeriodField = value;
        }
    }
}