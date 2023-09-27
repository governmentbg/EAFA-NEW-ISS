namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTPartyType
    {

        private IDType[] idField;

        private CodeType[] typeCodeField;

        private TextType nameField;

        private CodeType[] roleCodeField;

        private IDType residenceCountryIDField;

        private IDType[] typeIDField;

        private PartyContactType[] definedPartyContactField;

        private StructuredAddressType[] specifiedStructuredAddressField;

        private StructuredAddressType[] postalStructuredAddressField;

        private TechnicalCharacteristicType[] managedTechnicalCharacteristicField;

        private TTLocationType[] specifiedTTLocationField;

        private TTAnimalType[] specifiedTTAnimalField;

        private LocationResponsibleOrganizationType specifiedLocationResponsibleOrganizationField;

        private TTTransportMeansType[] managedTTTransportMeansField;

        private TTProductType[] specifiedTTProductField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("RoleCode")]
        public CodeType[] RoleCode
        {
            get => this.roleCodeField;
            set => this.roleCodeField = value;
        }

        public IDType ResidenceCountryID
        {
            get => this.residenceCountryIDField;
            set => this.residenceCountryIDField = value;
        }

        [XmlElement("TypeID")]
        public IDType[] TypeID
        {
            get => this.typeIDField;
            set => this.typeIDField = value;
        }

        [XmlElement("DefinedPartyContact")]
        public PartyContactType[] DefinedPartyContact
        {
            get => this.definedPartyContactField;
            set => this.definedPartyContactField = value;
        }

        [XmlElement("SpecifiedStructuredAddress")]
        public StructuredAddressType[] SpecifiedStructuredAddress
        {
            get => this.specifiedStructuredAddressField;
            set => this.specifiedStructuredAddressField = value;
        }

        [XmlElement("PostalStructuredAddress")]
        public StructuredAddressType[] PostalStructuredAddress
        {
            get => this.postalStructuredAddressField;
            set => this.postalStructuredAddressField = value;
        }

        [XmlElement("ManagedTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ManagedTechnicalCharacteristic
        {
            get => this.managedTechnicalCharacteristicField;
            set => this.managedTechnicalCharacteristicField = value;
        }

        [XmlElement("SpecifiedTTLocation")]
        public TTLocationType[] SpecifiedTTLocation
        {
            get => this.specifiedTTLocationField;
            set => this.specifiedTTLocationField = value;
        }

        [XmlElement("SpecifiedTTAnimal")]
        public TTAnimalType[] SpecifiedTTAnimal
        {
            get => this.specifiedTTAnimalField;
            set => this.specifiedTTAnimalField = value;
        }

        public LocationResponsibleOrganizationType SpecifiedLocationResponsibleOrganization
        {
            get => this.specifiedLocationResponsibleOrganizationField;
            set => this.specifiedLocationResponsibleOrganizationField = value;
        }

        [XmlElement("ManagedTTTransportMeans")]
        public TTTransportMeansType[] ManagedTTTransportMeans
        {
            get => this.managedTTTransportMeansField;
            set => this.managedTTTransportMeansField = value;
        }

        [XmlElement("SpecifiedTTProduct")]
        public TTProductType[] SpecifiedTTProduct
        {
            get => this.specifiedTTProductField;
            set => this.specifiedTTProductField = value;
        }
    }
}