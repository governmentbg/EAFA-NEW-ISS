namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportFinancialInstitution", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportFinancialInstitutionType
    {

        private IDType bEIIDField;

        private IDType bICIDField;

        private IDType gLNIDField;

        private IDType idField;

        private TextType nameField;

        private FinancialInstitutionRoleCodeType roleCodeField;

        public IDType BEIID
        {
            get
            {
                return this.bEIIDField;
            }
            set
            {
                this.bEIIDField = value;
            }
        }

        public IDType BICID
        {
            get
            {
                return this.bICIDField;
            }
            set
            {
                this.bICIDField = value;
            }
        }

        public IDType GLNID
        {
            get
            {
                return this.gLNIDField;
            }
            set
            {
                this.gLNIDField = value;
            }
        }

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

        public FinancialInstitutionRoleCodeType RoleCode
        {
            get
            {
                return this.roleCodeField;
            }
            set
            {
                this.roleCodeField = value;
            }
        }
    }
}