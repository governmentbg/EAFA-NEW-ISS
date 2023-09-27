namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTLocationType
    {

        private IDType idField;

        private TextType[] nameField;

        private LocationFunctionCodeType typeCodeField;

        private TextType[] descriptionField;

        private AnimalHoldingEventType[] specifiedAnimalHoldingEventField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        private TTPartyType responsibleTTPartyField;

        private TTAnimalType[] specifiedTTAnimalField;

        private GeographicalAreaType specifiedGeographicalAreaField;

        private TTProductProcessEventType[] specifiedTTProductProcessEventField;

        private TTProductType[] specifiedTTProductField;

        private StructuredAddressType specifiedStructuredAddressField;

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

        [XmlElement("Name")]
        public TextType[] Name
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

        public LocationFunctionCodeType TypeCode
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

        [XmlElement("Description")]
        public TextType[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        [XmlElement("SpecifiedAnimalHoldingEvent")]
        public AnimalHoldingEventType[] SpecifiedAnimalHoldingEvent
        {
            get
            {
                return this.specifiedAnimalHoldingEventField;
            }
            set
            {
                this.specifiedAnimalHoldingEventField = value;
            }
        }

        [XmlElement("ApplicableTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ApplicableTechnicalCharacteristic
        {
            get
            {
                return this.applicableTechnicalCharacteristicField;
            }
            set
            {
                this.applicableTechnicalCharacteristicField = value;
            }
        }

        public TTPartyType ResponsibleTTParty
        {
            get
            {
                return this.responsibleTTPartyField;
            }
            set
            {
                this.responsibleTTPartyField = value;
            }
        }

        [XmlElement("SpecifiedTTAnimal")]
        public TTAnimalType[] SpecifiedTTAnimal
        {
            get
            {
                return this.specifiedTTAnimalField;
            }
            set
            {
                this.specifiedTTAnimalField = value;
            }
        }

        public GeographicalAreaType SpecifiedGeographicalArea
        {
            get
            {
                return this.specifiedGeographicalAreaField;
            }
            set
            {
                this.specifiedGeographicalAreaField = value;
            }
        }

        [XmlElement("SpecifiedTTProductProcessEvent")]
        public TTProductProcessEventType[] SpecifiedTTProductProcessEvent
        {
            get
            {
                return this.specifiedTTProductProcessEventField;
            }
            set
            {
                this.specifiedTTProductProcessEventField = value;
            }
        }

        [XmlElement("SpecifiedTTProduct")]
        public TTProductType[] SpecifiedTTProduct
        {
            get
            {
                return this.specifiedTTProductField;
            }
            set
            {
                this.specifiedTTProductField = value;
            }
        }

        public StructuredAddressType SpecifiedStructuredAddress
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
    }
}