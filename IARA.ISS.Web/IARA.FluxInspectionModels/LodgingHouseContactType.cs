namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseContactType
    {

        private IDType idField;

        private TextType jobTitleField;

        private TextType responsibilityField;

        private TextType departmentNameField;

        private CodeType typeCodeField;

        private TextType personNameField;

        private IndicatorType primaryIndicatorField;

        private TextType descriptionField;

        private UniversalCommunicationType[] telephoneUniversalCommunicationField;

        private StructuredAddressType[] postalStructuredAddressField;

        private SpecifiedPeriodType[] availableSpecifiedPeriodField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType JobTitle
        {
            get => this.jobTitleField;
            set => this.jobTitleField = value;
        }

        public TextType Responsibility
        {
            get => this.responsibilityField;
            set => this.responsibilityField = value;
        }

        public TextType DepartmentName
        {
            get => this.departmentNameField;
            set => this.departmentNameField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType PersonName
        {
            get => this.personNameField;
            set => this.personNameField = value;
        }

        public IndicatorType PrimaryIndicator
        {
            get => this.primaryIndicatorField;
            set => this.primaryIndicatorField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("TelephoneUniversalCommunication")]
        public UniversalCommunicationType[] TelephoneUniversalCommunication
        {
            get => this.telephoneUniversalCommunicationField;
            set => this.telephoneUniversalCommunicationField = value;
        }

        [XmlElement("PostalStructuredAddress")]
        public StructuredAddressType[] PostalStructuredAddress
        {
            get => this.postalStructuredAddressField;
            set => this.postalStructuredAddressField = value;
        }

        [XmlElement("AvailableSpecifiedPeriod")]
        public SpecifiedPeriodType[] AvailableSpecifiedPeriod
        {
            get => this.availableSpecifiedPeriodField;
            set => this.availableSpecifiedPeriodField = value;
        }
    }
}