namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedPartyType
    {

        private IDType idField;

        private TextType nameField;

        private IDType countryIDField;

        private PartyContactType definedPartyContactField;

        private UnstructuredAddressType officeUnstructuredAddressField;

        private StructuredAddressType[] specifiedStructuredAddressField;

        private SpecifiedCommunicationType[] telephoneSpecifiedCommunicationField;

        private SpecifiedCommunicationType[] uRISpecifiedCommunicationField;

        private OperatorPersonType[] specifiedOperatorPersonField;

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

        public IDType CountryID
        {
            get
            {
                return this.countryIDField;
            }
            set
            {
                this.countryIDField = value;
            }
        }

        public PartyContactType DefinedPartyContact
        {
            get
            {
                return this.definedPartyContactField;
            }
            set
            {
                this.definedPartyContactField = value;
            }
        }

        public UnstructuredAddressType OfficeUnstructuredAddress
        {
            get
            {
                return this.officeUnstructuredAddressField;
            }
            set
            {
                this.officeUnstructuredAddressField = value;
            }
        }

        [XmlElement("SpecifiedStructuredAddress")]
        public StructuredAddressType[] SpecifiedStructuredAddress
        {
            get
            {
                return this.specifiedStructuredAddressField;
            }
            set
            {
                this.specifiedStructuredAddressField = value;
            }
        }

        [XmlElement("TelephoneSpecifiedCommunication")]
        public SpecifiedCommunicationType[] TelephoneSpecifiedCommunication
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

        [XmlElement("URISpecifiedCommunication")]
        public SpecifiedCommunicationType[] URISpecifiedCommunication
        {
            get
            {
                return this.uRISpecifiedCommunicationField;
            }
            set
            {
                this.uRISpecifiedCommunicationField = value;
            }
        }

        [XmlElement("SpecifiedOperatorPerson")]
        public OperatorPersonType[] SpecifiedOperatorPerson
        {
            get
            {
                return this.specifiedOperatorPersonField;
            }
            set
            {
                this.specifiedOperatorPersonField = value;
            }
        }
    }
}