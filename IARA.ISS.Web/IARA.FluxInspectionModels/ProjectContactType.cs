namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectContactType
    {

        private IDType idField;

        private TextType jobTitleField;

        private TextType responsibilityField;

        private TextType departmentNameField;

        private CodeType typeCodeField;

        private TextType personNameField;

        private TelecommunicationCommunicationType[] telephoneTelecommunicationCommunicationField;

        private SpecifiedPreferenceType usageSpecifiedPreferenceField;

        private UnstructuredAddressType[] postalUnstructuredAddressField;

        private TelecommunicationCommunicationType faxTelecommunicationCommunicationField;

        private InternetCommunicationType[] uRIInternetCommunicationField;

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

        public CodeType TypeCode
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

        public TextType PersonName
        {
            get
            {
                return this.personNameField;
            }
            set
            {
                this.personNameField = value;
            }
        }

        [XmlElement("TelephoneTelecommunicationCommunication")]
        public TelecommunicationCommunicationType[] TelephoneTelecommunicationCommunication
        {
            get
            {
                return this.telephoneTelecommunicationCommunicationField;
            }
            set
            {
                this.telephoneTelecommunicationCommunicationField = value;
            }
        }

        public SpecifiedPreferenceType UsageSpecifiedPreference
        {
            get
            {
                return this.usageSpecifiedPreferenceField;
            }
            set
            {
                this.usageSpecifiedPreferenceField = value;
            }
        }

        [XmlElement("PostalUnstructuredAddress")]
        public UnstructuredAddressType[] PostalUnstructuredAddress
        {
            get
            {
                return this.postalUnstructuredAddressField;
            }
            set
            {
                this.postalUnstructuredAddressField = value;
            }
        }

        public TelecommunicationCommunicationType FaxTelecommunicationCommunication
        {
            get
            {
                return this.faxTelecommunicationCommunicationField;
            }
            set
            {
                this.faxTelecommunicationCommunicationField = value;
            }
        }

        [XmlElement("URIInternetCommunication")]
        public InternetCommunicationType[] URIInternetCommunication
        {
            get
            {
                return this.uRIInternetCommunicationField;
            }
            set
            {
                this.uRIInternetCommunicationField = value;
            }
        }
    }
}