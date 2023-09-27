namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PartyContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PartyContactType
    {

        private IDType idField;

        private TextType departmentNameField;

        private TextType personNameField;

        private TextType jobTitleField;

        private IDType additionalIDField;

        private UnstructuredTelecommunicationCommunicationType directTelephoneUnstructuredTelecommunicationCommunicationField;

        private UnstructuredTelecommunicationCommunicationType mobileTelephoneUnstructuredTelecommunicationCommunicationField;

        private UnstructuredTelecommunicationCommunicationType faxUnstructuredTelecommunicationCommunicationField;

        private EmailCommunicationType uRIEmailCommunicationField;

        private UnstructuredTelecommunicationCommunicationType telexUnstructuredTelecommunicationCommunicationField;

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

        public IDType AdditionalID
        {
            get
            {
                return this.additionalIDField;
            }
            set
            {
                this.additionalIDField = value;
            }
        }

        public UnstructuredTelecommunicationCommunicationType DirectTelephoneUnstructuredTelecommunicationCommunication
        {
            get
            {
                return this.directTelephoneUnstructuredTelecommunicationCommunicationField;
            }
            set
            {
                this.directTelephoneUnstructuredTelecommunicationCommunicationField = value;
            }
        }

        public UnstructuredTelecommunicationCommunicationType MobileTelephoneUnstructuredTelecommunicationCommunication
        {
            get
            {
                return this.mobileTelephoneUnstructuredTelecommunicationCommunicationField;
            }
            set
            {
                this.mobileTelephoneUnstructuredTelecommunicationCommunicationField = value;
            }
        }

        public UnstructuredTelecommunicationCommunicationType FaxUnstructuredTelecommunicationCommunication
        {
            get
            {
                return this.faxUnstructuredTelecommunicationCommunicationField;
            }
            set
            {
                this.faxUnstructuredTelecommunicationCommunicationField = value;
            }
        }

        public EmailCommunicationType URIEmailCommunication
        {
            get
            {
                return this.uRIEmailCommunicationField;
            }
            set
            {
                this.uRIEmailCommunicationField = value;
            }
        }

        public UnstructuredTelecommunicationCommunicationType TelexUnstructuredTelecommunicationCommunication
        {
            get
            {
                return this.telexUnstructuredTelecommunicationCommunicationField;
            }
            set
            {
                this.telexUnstructuredTelecommunicationCommunicationField = value;
            }
        }
    }
}