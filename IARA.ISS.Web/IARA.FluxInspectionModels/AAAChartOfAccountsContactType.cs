namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAChartOfAccountsContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAChartOfAccountsContactType
    {

        private IDType idField;

        private TextType jobTitleField;

        private TextType responsibilityField;

        private TextType departmentNameField;

        private AccountingContactCodeType typeCodeField;

        private AAAChartOfAccountsPersonType[] specifiedAAAChartOfAccountsPersonField;

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

        public TextType JobTitle
        {
            get
            {
                return this.jobTitleField;
            }
            set
            {
                this.jobTitleField = value;
            }
        }

        public TextType Responsibility
        {
            get
            {
                return this.responsibilityField;
            }
            set
            {
                this.responsibilityField = value;
            }
        }

        public TextType DepartmentName
        {
            get
            {
                return this.departmentNameField;
            }
            set
            {
                this.departmentNameField = value;
            }
        }

        public AccountingContactCodeType TypeCode
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

        [XmlElement("SpecifiedAAAChartOfAccountsPerson")]
        public AAAChartOfAccountsPersonType[] SpecifiedAAAChartOfAccountsPerson
        {
            get
            {
                return this.specifiedAAAChartOfAccountsPersonField;
            }
            set
            {
                this.specifiedAAAChartOfAccountsPersonField = value;
            }
        }
    }
}