namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportFormality", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportFormalityType
    {

        private IDType idField;

        private TextType nameField;

        private TextType[] manifestField;

        private IDType nomenclatureIDField;

        private TextType nomenclatureNameField;

        private AAAReportFormTemplateType[] includedAAAReportFormTemplateField;

        private AAAReportOrganizationType concernedAAAReportOrganizationField;

        private AAAReportAccountingPeriodType[] specifiedAAAReportAccountingPeriodField;

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

        [XmlElement("Manifest")]
        public TextType[] Manifest
        {
            get
            {
                return this.manifestField;
            }
            set
            {
                this.manifestField = value;
            }
        }

        public IDType NomenclatureID
        {
            get
            {
                return this.nomenclatureIDField;
            }
            set
            {
                this.nomenclatureIDField = value;
            }
        }

        public TextType NomenclatureName
        {
            get
            {
                return this.nomenclatureNameField;
            }
            set
            {
                this.nomenclatureNameField = value;
            }
        }

        [XmlElement("IncludedAAAReportFormTemplate")]
        public AAAReportFormTemplateType[] IncludedAAAReportFormTemplate
        {
            get
            {
                return this.includedAAAReportFormTemplateField;
            }
            set
            {
                this.includedAAAReportFormTemplateField = value;
            }
        }

        public AAAReportOrganizationType ConcernedAAAReportOrganization
        {
            get
            {
                return this.concernedAAAReportOrganizationField;
            }
            set
            {
                this.concernedAAAReportOrganizationField = value;
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
    }
}