namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapContactType
    {

        private TextType jobTitleField;

        private IDType idField;

        private TextType departmentNameField;

        private TextType responsibilityField;

        private AccountingContactCodeType typeCodeField;

        private AAAWrapPersonType[] specifiedAAAWrapPersonField;

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

        [XmlElement("SpecifiedAAAWrapPerson")]
        public AAAWrapPersonType[] SpecifiedAAAWrapPerson
        {
            get
            {
                return this.specifiedAAAWrapPersonField;
            }
            set
            {
                this.specifiedAAAWrapPersonField = value;
            }
        }
    }
}