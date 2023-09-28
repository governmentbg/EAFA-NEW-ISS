namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingContactType
    {

        private IDType idField;

        private TextType jobTitleField;

        private TextType responsibilityField;

        private TextType departmentNameField;

        private TextType governmentDepartmentNameField;

        private TextType bureauDepartmentNameField;

        private TextType divisionDepartmentNameField;

        private TextType officeCounterDepartmentNameField;

        private TextType personNameField;

        private TenderingCommunicationType[] telephoneTenderingCommunicationField;

        private TenderingAddressType[] postalTenderingAddressField;

        private TenderingCommunicationType[] faxTenderingCommunicationField;

        private TenderingCommunicationType[] uRITenderingCommunicationField;

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

        public TextType GovernmentDepartmentName
        {
            get
            {
                return this.governmentDepartmentNameField;
            }
            set
            {
                this.governmentDepartmentNameField = value;
            }
        }

        public TextType BureauDepartmentName
        {
            get
            {
                return this.bureauDepartmentNameField;
            }
            set
            {
                this.bureauDepartmentNameField = value;
            }
        }

        public TextType DivisionDepartmentName
        {
            get
            {
                return this.divisionDepartmentNameField;
            }
            set
            {
                this.divisionDepartmentNameField = value;
            }
        }

        public TextType OfficeCounterDepartmentName
        {
            get
            {
                return this.officeCounterDepartmentNameField;
            }
            set
            {
                this.officeCounterDepartmentNameField = value;
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

        [XmlElement("TelephoneTenderingCommunication")]
        public TenderingCommunicationType[] TelephoneTenderingCommunication
        {
            get
            {
                return this.telephoneTenderingCommunicationField;
            }
            set
            {
                this.telephoneTenderingCommunicationField = value;
            }
        }

        [XmlElement("PostalTenderingAddress")]
        public TenderingAddressType[] PostalTenderingAddress
        {
            get
            {
                return this.postalTenderingAddressField;
            }
            set
            {
                this.postalTenderingAddressField = value;
            }
        }

        [XmlElement("FaxTenderingCommunication")]
        public TenderingCommunicationType[] FaxTenderingCommunication
        {
            get
            {
                return this.faxTenderingCommunicationField;
            }
            set
            {
                this.faxTenderingCommunicationField = value;
            }
        }

        [XmlElement("URITenderingCommunication")]
        public TenderingCommunicationType[] URITenderingCommunication
        {
            get
            {
                return this.uRITenderingCommunicationField;
            }
            set
            {
                this.uRITenderingCommunicationField = value;
            }
        }
    }
}