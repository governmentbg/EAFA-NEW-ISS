namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LaboratoryObservationContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LaboratoryObservationContactType
    {

        private IDType idField;

        private TextType personNameField;

        private TextType departmentNameField;

        private SpecifiedCommunicationType mobileTelephoneSpecifiedCommunicationField;

        private SpecifiedCommunicationType telephoneSpecifiedCommunicationField;

        private SpecifiedCommunicationType faxSpecifiedCommunicationField;

        private SpecifiedCommunicationType emailSpecifiedCommunicationField;

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

        public SpecifiedCommunicationType MobileTelephoneSpecifiedCommunication
        {
            get
            {
                return this.mobileTelephoneSpecifiedCommunicationField;
            }
            set
            {
                this.mobileTelephoneSpecifiedCommunicationField = value;
            }
        }

        public SpecifiedCommunicationType TelephoneSpecifiedCommunication
        {
            get
            {
                return this.telephoneSpecifiedCommunicationField;
            }
            set
            {
                this.telephoneSpecifiedCommunicationField = value;
            }
        }

        public SpecifiedCommunicationType FaxSpecifiedCommunication
        {
            get
            {
                return this.faxSpecifiedCommunicationField;
            }
            set
            {
                this.faxSpecifiedCommunicationField = value;
            }
        }

        public SpecifiedCommunicationType EmailSpecifiedCommunication
        {
            get
            {
                return this.emailSpecifiedCommunicationField;
            }
            set
            {
                this.emailSpecifiedCommunicationField = value;
            }
        }
    }
}