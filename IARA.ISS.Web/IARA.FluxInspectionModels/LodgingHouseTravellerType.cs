namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseTraveller", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseTravellerType
    {

        private IDType sequenceIDField;

        private CodeType categoryCodeField;

        private CodeType reservationStatusCodeField;

        private IndicatorType leaderIndicatorField;

        private TextType roomingField;

        private TextType descriptionField;

        private LodgingHouseTravelProductType requestedLodgingHouseTravelProductField;

        private TravelGuestArrivalType providedTravelGuestArrivalField;

        private SpecialQueryType[] submittedSpecialQueryField;

        private GuestPersonType informationProvidedGuestPersonField;

        public IDType SequenceID
        {
            get => this.sequenceIDField;
            set => this.sequenceIDField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public CodeType ReservationStatusCode
        {
            get => this.reservationStatusCodeField;
            set => this.reservationStatusCodeField = value;
        }

        public IndicatorType LeaderIndicator
        {
            get => this.leaderIndicatorField;
            set => this.leaderIndicatorField = value;
        }

        public TextType Rooming
        {
            get => this.roomingField;
            set => this.roomingField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public LodgingHouseTravelProductType RequestedLodgingHouseTravelProduct
        {
            get => this.requestedLodgingHouseTravelProductField;
            set => this.requestedLodgingHouseTravelProductField = value;
        }

        public TravelGuestArrivalType ProvidedTravelGuestArrival
        {
            get => this.providedTravelGuestArrivalField;
            set => this.providedTravelGuestArrivalField = value;
        }

        [XmlElement("SubmittedSpecialQuery")]
        public SpecialQueryType[] SubmittedSpecialQuery
        {
            get => this.submittedSpecialQueryField;
            set => this.submittedSpecialQueryField = value;
        }

        public GuestPersonType InformationProvidedGuestPerson
        {
            get => this.informationProvidedGuestPersonField;
            set => this.informationProvidedGuestPersonField = value;
        }
    }
}