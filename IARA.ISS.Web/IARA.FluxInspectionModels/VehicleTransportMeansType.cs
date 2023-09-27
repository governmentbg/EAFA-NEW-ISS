namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VehicleTransportMeans", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VehicleTransportMeansType
    {

        private IDType idField;

        private IDType registrationCountryIDField;

        private TextType nameField;

        private CodeType typeCodeField;

        private CodeType roleCodeField;

        private IDType[] transportEquipmentIDField;

        private SalesPartyType ownerSalesPartyField;

        private ContactPartyType[] specifiedContactPartyField;

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

        public IDType RegistrationCountryID
        {
            get
            {
                return this.registrationCountryIDField;
            }
            set
            {
                this.registrationCountryIDField = value;
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

        public CodeType RoleCode
        {
            get
            {
                return this.roleCodeField;
            }
            set
            {
                this.roleCodeField = value;
            }
        }

        [XmlElement("TransportEquipmentID")]
        public IDType[] TransportEquipmentID
        {
            get
            {
                return this.transportEquipmentIDField;
            }
            set
            {
                this.transportEquipmentIDField = value;
            }
        }

        public SalesPartyType OwnerSalesParty
        {
            get
            {
                return this.ownerSalesPartyField;
            }
            set
            {
                this.ownerSalesPartyField = value;
            }
        }

        [XmlElement("SpecifiedContactParty")]
        public ContactPartyType[] SpecifiedContactParty
        {
            get
            {
                return this.specifiedContactPartyField;
            }
            set
            {
                this.specifiedContactPartyField = value;
            }
        }
    }
}