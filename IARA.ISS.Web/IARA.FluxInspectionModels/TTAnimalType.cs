namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTAnimal", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTAnimalType
    {

        private CodeType speciesTypeCodeField;

        private TTPartyType holderResponsibleTTPartyField;

        private AnimalTradeDeliveryType applicableAnimalTradeDeliveryField;

        private TTTransportMovementType[][] relatedTTConsignmentField;

        private AnimalBatchType specifiedAnimalBatchField;

        private AnimalIdentityType[] specifiedAnimalIdentityField;

        private IndividualTTAnimalType specifiedIndividualTTAnimalField;

        private DelimitedPeriodType[] specifiedDelimitedPeriodField;

        private AnimalHoldingEventType[] specifiedAnimalHoldingEventField;

        private AnimalCertificateType[] specifiedAnimalCertificateField;

        private SpeciesTTAnimalType[] specifiedSpeciesTTAnimalField;

        private TTLocationType[] relatedTTLocationField;

        public CodeType SpeciesTypeCode
        {
            get
            {
                return this.speciesTypeCodeField;
            }
            set
            {
                this.speciesTypeCodeField = value;
            }
        }

        public TTPartyType HolderResponsibleTTParty
        {
            get
            {
                return this.holderResponsibleTTPartyField;
            }
            set
            {
                this.holderResponsibleTTPartyField = value;
            }
        }

        public AnimalTradeDeliveryType ApplicableAnimalTradeDelivery
        {
            get
            {
                return this.applicableAnimalTradeDeliveryField;
            }
            set
            {
                this.applicableAnimalTradeDeliveryField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("SpecifiedTTTransportMovement", typeof(TTTransportMovementType), IsNullable = false)]
        public TTTransportMovementType[][] RelatedTTConsignment
        {
            get
            {
                return this.relatedTTConsignmentField;
            }
            set
            {
                this.relatedTTConsignmentField = value;
            }
        }

        public AnimalBatchType SpecifiedAnimalBatch
        {
            get
            {
                return this.specifiedAnimalBatchField;
            }
            set
            {
                this.specifiedAnimalBatchField = value;
            }
        }

        [XmlElement("SpecifiedAnimalIdentity")]
        public AnimalIdentityType[] SpecifiedAnimalIdentity
        {
            get
            {
                return this.specifiedAnimalIdentityField;
            }
            set
            {
                this.specifiedAnimalIdentityField = value;
            }
        }

        public IndividualTTAnimalType SpecifiedIndividualTTAnimal
        {
            get
            {
                return this.specifiedIndividualTTAnimalField;
            }
            set
            {
                this.specifiedIndividualTTAnimalField = value;
            }
        }

        [XmlElement("SpecifiedDelimitedPeriod")]
        public DelimitedPeriodType[] SpecifiedDelimitedPeriod
        {
            get
            {
                return this.specifiedDelimitedPeriodField;
            }
            set
            {
                this.specifiedDelimitedPeriodField = value;
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

        [XmlElement("SpecifiedAnimalCertificate")]
        public AnimalCertificateType[] SpecifiedAnimalCertificate
        {
            get
            {
                return this.specifiedAnimalCertificateField;
            }
            set
            {
                this.specifiedAnimalCertificateField = value;
            }
        }

        [XmlElement("SpecifiedSpeciesTTAnimal")]
        public SpeciesTTAnimalType[] SpecifiedSpeciesTTAnimal
        {
            get
            {
                return this.specifiedSpeciesTTAnimalField;
            }
            set
            {
                this.specifiedSpeciesTTAnimalField = value;
            }
        }

        [XmlElement("RelatedTTLocation")]
        public TTLocationType[] RelatedTTLocation
        {
            get
            {
                return this.relatedTTLocationField;
            }
            set
            {
                this.relatedTTLocationField = value;
            }
        }
    }
}