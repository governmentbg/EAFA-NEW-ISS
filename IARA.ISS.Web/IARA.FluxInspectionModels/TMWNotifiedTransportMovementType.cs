namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWNotifiedTransportMovement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWNotifiedTransportMovementType
    {

        private QuantityType totalQuantityField;

        private IDType notificationIDField;

        private TextType reasonField;

        private DateTimeType firstStartDateTimeField;

        private DateTimeType lastStartDateTimeField;

        private TMWRouteType itineraryTMWRouteField;

        private TMWRouteType[] alternativeTMWRouteField;

        private TMWSingleTransportMovementType[] sectionTMWSingleTransportMovementField;

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

        public TextType Reason
        {
            get => this.reasonField;
            set => this.reasonField = value;
        }

        public DateTimeType FirstStartDateTime
        {
            get => this.firstStartDateTimeField;
            set => this.firstStartDateTimeField = value;
        }

        public DateTimeType LastStartDateTime
        {
            get => this.lastStartDateTimeField;
            set => this.lastStartDateTimeField = value;
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