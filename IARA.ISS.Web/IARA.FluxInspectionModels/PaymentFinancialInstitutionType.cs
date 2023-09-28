namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PaymentFinancialInstitution", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PaymentFinancialInstitutionType
    {

        private IDType bEIIDField;

        private IDType bICIDField;

        private IDType gLNIDField;

        private IDType idField;

        private TextType[] nameField;

        private CodeType roleCodeField;

        public IDType BEIID
        {
            get => this.bEIIDField;
            set => this.bEIIDField = value;
        }

        public IDType BICID
        {
            get => this.bICIDField;
            set => this.bICIDField = value;
        }

        public IDType GLNID
        {
            get => this.gLNIDField;
            set => this.gLNIDField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public CodeType RoleCode
        {
            get => this.roleCodeField;
            set => this.roleCodeField = value;
        }
    }
}