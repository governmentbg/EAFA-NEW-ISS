namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnimalTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnimalTradeDeliveryType
    {

        private IDType idField;

        private TTPartyType responsibleTTPartyField;

        private TTAnimalType[] includedTTAnimalField;

        private TTTransportMovementType[][] relatedTTConsignmentField;

        private TTEventType[] deliveryTTEventField;

        private TTEventType[] despatchTTEventField;

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

        [XmlElement("IncludedTTAnimal")]
        public TTAnimalType[] IncludedTTAnimal
        {
            get
            {
                return this.includedTTAnimalField;
            }
            set
            {
                this.includedTTAnimalField = value;
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

        [XmlElement("DeliveryTTEvent")]
        public TTEventType[] DeliveryTTEvent
        {
            get
            {
                return this.deliveryTTEventField;
            }
            set
            {
                this.deliveryTTEventField = value;
            }
        }

        [XmlElement("DespatchTTEvent")]
        public TTEventType[] DespatchTTEvent
        {
            get
            {
                return this.despatchTTEventField;
            }
            set
            {
                this.despatchTTEventField = value;
            }
        }
    }
}