namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LaboratoryObservationParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LaboratoryObservationPartyType
    {

        private IDType idField;

        private TextType nameField;

        private IDType[] thirdPartyIssuedIDField;

        private TextType[] thirdPartyIssuedIdentificationField;

        private StructuredAddressType officeStructuredAddressField;

        private StructuredAddressType postalStructuredAddressField;

        private SpecifiedCommunicationType websiteURISpecifiedCommunicationField;

        private LaboratoryObservationContactType personDefinedLaboratoryObservationContactField;

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

        [XmlElement("ThirdPartyIssuedID")]
        public IDType[] ThirdPartyIssuedID
        {
            get
            {
                return this.thirdPartyIssuedIDField;
            }
            set
            {
                this.thirdPartyIssuedIDField = value;
            }
        }

        [XmlElement("ThirdPartyIssuedIdentification")]
        public TextType[] ThirdPartyIssuedIdentification
        {
            get
            {
                return this.thirdPartyIssuedIdentificationField;
            }
            set
            {
                this.thirdPartyIssuedIdentificationField = value;
            }
        }

        public StructuredAddressType OfficeStructuredAddress
        {
            get
            {
                return this.officeStructuredAddressField;
            }
            set
            {
                this.officeStructuredAddressField = value;
            }
        }

        public StructuredAddressType PostalStructuredAddress
        {
            get
            {
                return this.postalStructuredAddressField;
            }
            set
            {
                this.postalStructuredAddressField = value;
            }
        }

        public SpecifiedCommunicationType WebsiteURISpecifiedCommunication
        {
            get
            {
                return this.websiteURISpecifiedCommunicationField;
            }
            set
            {
                this.websiteURISpecifiedCommunicationField = value;
            }
        }

        public LaboratoryObservationContactType PersonDefinedLaboratoryObservationContact
        {
            get
            {
                return this.personDefinedLaboratoryObservationContactField;
            }
            set
            {
                this.personDefinedLaboratoryObservationContactField = value;
            }
        }
    }
}