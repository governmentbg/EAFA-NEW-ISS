namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWAnnouncedTransportMovement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWAnnouncedTransportMovementType
    {

        private NumericType sequenceNumericField;

        private QuantityType totalQuantityField;

        private IDType notificationIDField;

        private DateTimeType startDateTimeField;

        private TMWRouteType itineraryTMWRouteField;

        private TMWRouteType[] alternativeTMWRouteField;

        private TMWSingleTransportMovementType[] sectionTMWSingleTransportMovementField;

        public NumericType SequenceNumeric
        {
            get => this.sequenceNumericField;
            set => this.sequenceNumericField = value;
        }

        public QuantityType TotalQuantity
        {
            get => this.totalQuantityField;
            set => this.totalQuantityField = value;
        }

        public IDType NotificationID
        {
            get => this.notificationIDField;
            set => this.notificationIDField = value;
        }

        public DateTimeType StartDateTime
        {
            get => this.startDateTimeField;
            set => this.startDateTimeField = value;
        }

        public TMWRouteType ItineraryTMWRoute
        {
            get => this.itineraryTMWRouteField;
            set => this.itineraryTMWRouteField = value;
        }

        [XmlElement("AlternativeTMWRoute")]
        public TMWRouteType[] AlternativeTMWRoute
        {
            get => this.alternativeTMWRouteField;
            set => this.alternativeTMWRouteField = value;
        }

        [XmlElement("SectionTMWSingleTransportMovement")]
        public TMWSingleTransportMovementType[] SectionTMWSingleTransportMovement
        {
            get => this.sectionTMWSingleTransportMovementField;
            set => this.sectionTMWSingleTransportMovementField = value;
        }
    }
}