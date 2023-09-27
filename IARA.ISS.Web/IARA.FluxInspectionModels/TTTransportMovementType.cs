namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTTransportMovement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTTransportMovementType
    {

        private TTEventType[] arrivalTTEventField;

        private TTEventType[] departureTTEventField;

        private TTEventType[] intermediateTTEventField;

        private TTTransportMeansType usedTTTransportMeansField;

        private TTPartyType carrierTTPartyField;

        [XmlElement("ArrivalTTEvent")]
        public TTEventType[] ArrivalTTEvent
        {
            get
            {
                return this.arrivalTTEventField;
            }
            set
            {
                this.arrivalTTEventField = value;
            }
        }

        [XmlElement("DepartureTTEvent")]
        public TTEventType[] DepartureTTEvent
        {
            get
            {
                return this.departureTTEventField;
            }
            set
            {
                this.departureTTEventField = value;
            }
        }

        [XmlElement("IntermediateTTEvent")]
        public TTEventType[] IntermediateTTEvent
        {
            get
            {
                return this.intermediateTTEventField;
            }
            set
            {
                this.intermediateTTEventField = value;
            }
        }

        public TTTransportMeansType UsedTTTransportMeans
        {
            get
            {
                return this.usedTTTransportMeansField;
            }
            set
            {
                this.usedTTTransportMeansField = value;
            }
        }

        public TTPartyType CarrierTTParty
        {
            get
            {
                return this.carrierTTPartyField;
            }
            set
            {
                this.carrierTTPartyField = value;
            }
        }
    }
}