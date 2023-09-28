namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeContactType
    {

        private IDType idField;

        private TextType personNameField;

        private TextType departmentNameField;

        private ContactTypeCodeType typeCodeField;

        private TextType jobTitleField;

        private TextType responsibilityField;

        private IDType[] personIDField;

        private TextType authorizedPersonNameField;

        private CIUniversalCommunicationType telephoneCIUniversalCommunicationField;

        private CIUniversalCommunicationType directTelephoneCIUniversalCommunicationField;

        private CIUniversalCommunicationType mobileTelephoneCIUniversalCommunicationField;

        private CIUniversalCommunicationType faxCIUniversalCommunicationField;

        private CIUniversalCommunicationType emailURICIUniversalCommunicationField;

        private CIUniversalCommunicationType telexCIUniversalCommunicationField;

        private CIUniversalCommunicationType vOIPCIUniversalCommunicationField;

        private CIUniversalCommunicationType instantMessagingCIUniversalCommunicationField;

        private CINoteType[] specifiedCINoteField;

        private CIContactPersonType specifiedCIContactPersonField;

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

        public ContactTypeCodeType TypeCode
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

        [XmlElement("PersonID")]
        public IDType[] PersonID
        {
            get
            {
                return this.personIDField;
            }
            set
            {
                this.personIDField = value;
            }
        }

        public TextType AuthorizedPersonName
        {
            get
            {
                return this.authorizedPersonNameField;
            }
            set
            {
                this.authorizedPersonNameField = value;
            }
        }

        public CIUniversalCommunicationType TelephoneCIUniversalCommunication
        {
            get
            {
                return this.telephoneCIUniversalCommunicationField;
            }
            set
            {
                this.telephoneCIUniversalCommunicationField = value;
            }
        }

        public CIUniversalCommunicationType DirectTelephoneCIUniversalCommunication
        {
            get
            {
                return this.directTelephoneCIUniversalCommunicationField;
            }
            set
            {
                this.directTelephoneCIUniversalCommunicationField = value;
            }
        }

        public CIUniversalCommunicationType MobileTelephoneCIUniversalCommunication
        {
            get
            {
                return this.mobileTelephoneCIUniversalCommunicationField;
            }
            set
            {
                this.mobileTelephoneCIUniversalCommunicationField = value;
            }
        }

        public CIUniversalCommunicationType FaxCIUniversalCommunication
        {
            get
            {
                return this.faxCIUniversalCommunicationField;
            }
            set
            {
                this.faxCIUniversalCommunicationField = value;
            }
        }

        public CIUniversalCommunicationType EmailURICIUniversalCommunication
        {
            get
            {
                return this.emailURICIUniversalCommunicationField;
            }
            set
            {
                this.emailURICIUniversalCommunicationField = value;
            }
        }

        public CIUniversalCommunicationType TelexCIUniversalCommunication
        {
            get
            {
                return this.telexCIUniversalCommunicationField;
            }
            set
            {
                this.telexCIUniversalCommunicationField = value;
            }
        }

        public CIUniversalCommunicationType VOIPCIUniversalCommunication
        {
            get
            {
                return this.vOIPCIUniversalCommunicationField;
            }
            set
            {
                this.vOIPCIUniversalCommunicationField = value;
            }
        }

        public CIUniversalCommunicationType InstantMessagingCIUniversalCommunication
        {
            get
            {
                return this.instantMessagingCIUniversalCommunicationField;
            }
            set
            {
                this.instantMessagingCIUniversalCommunicationField = value;
            }
        }

        [XmlElement("SpecifiedCINote")]
        public CINoteType[] SpecifiedCINote
        {
            get
            {
                return this.specifiedCINoteField;
            }
            set
            {
                this.specifiedCINoteField = value;
            }
        }

        public CIContactPersonType SpecifiedCIContactPerson
        {
            get
            {
                return this.specifiedCIContactPersonField;
            }
            set
            {
                this.specifiedCIContactPersonField = value;
            }
        }
    }
}