namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingAssignment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingAssignmentType
    {

        private IDType idField;

        private TextType[] descriptionField;

        private TenderingPeriodType effectiveTenderingPeriodField;

        private ProcuringProjectType limitedProcuringProjectField;

        private ApplicationOrganizationType delegatedApplicationOrganizationField;

        private ApplicationOrganizationType delegatingApplicationOrganizationField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TenderingPeriodType EffectiveTenderingPeriod
        {
            get => this.effectiveTenderingPeriodField;
            set => this.effectiveTenderingPeriodField = value;
        }

        public ProcuringProjectType LimitedProcuringProject
        {
            get => this.limitedProcuringProjectField;
            set => this.limitedProcuringProjectField = value;
        }

        public ApplicationOrganizationType DelegatedApplicationOrganization
        {
            get => this.delegatedApplicationOrganizationField;
            set => this.delegatedApplicationOrganizationField = value;
        }

        public ApplicationOrganizationType DelegatingApplicationOrganization
        {
            get => this.delegatingApplicationOrganizationField;
            set => this.delegatingApplicationOrganizationField = value;
        }
    }
}