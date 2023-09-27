namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseReservation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseReservationType
    {

        private CodeType messageTypeCodeField;

        private IDType idField;

        private DateTimeType acceptedDateTimeField;

        private CodeType statusCodeField;

        private QuantityType totalCustomerQuantityField;

        private IndicatorType salesPolicyAcceptedIndicatorField;

        private IDType propertyConfirmationIDField;

        private DateTimeType propertyConfirmationIssuedDateTimeField;

        private IDType cancellationIDField;

        private DateTimeType cancellationAcceptedDateTimeField;

        private TextType descriptionField;

        private LodgingHouseTravellerType[] specifiedLodgingHouseTravellerField;

        private TravelSalesChannelType[] usedTravelSalesChannelField;

        private ReservingPersonType makingReservingPersonField;

        private TravelPaymentType[] applicableTravelPaymentField;

        public CodeType MessageTypeCode
        {
            get => this.messageTypeCodeField;
            set => this.messageTypeCodeField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public DateTimeType AcceptedDateTime
        {
            get => this.acceptedDateTimeField;
            set => this.acceptedDateTimeField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public QuantityType TotalCustomerQuantity
        {
            get => this.totalCustomerQuantityField;
            set => this.totalCustomerQuantityField = value;
        }

        public IndicatorType SalesPolicyAcceptedIndicator
        {
            get => this.salesPolicyAcceptedIndicatorField;
            set => this.salesPolicyAcceptedIndicatorField = value;
        }

        public IDType PropertyConfirmationID
        {
            get => this.propertyConfirmationIDField;
            set => this.propertyConfirmationIDField = value;
        }

        public DateTimeType PropertyConfirmationIssuedDateTime
        {
            get => this.propertyConfirmationIssuedDateTimeField;
            set => this.propertyConfirmationIssuedDateTimeField = value;
        }

        public IDType CancellationID
        {
            get => this.cancellationIDField;
            set => this.cancellationIDField = value;
        }

        public DateTimeType CancellationAcceptedDateTime
        {
            get => this.cancellationAcceptedDateTimeField;
            set => this.cancellationAcceptedDateTimeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("SpecifiedLodgingHouseTraveller")]
        public LodgingHouseTravellerType[] SpecifiedLodgingHouseTraveller
        {
            get => this.specifiedLodgingHouseTravellerField;
            set => this.specifiedLodgingHouseTravellerField = value;
        }

        [XmlElement("UsedTravelSalesChannel")]
        public TravelSalesChannelType[] UsedTravelSalesChannel
        {
            get => this.usedTravelSalesChannelField;
            set => this.usedTravelSalesChannelField = value;
        }

        public ReservingPersonType MakingReservingPerson
        {
            get => this.makingReservingPersonField;
            set => this.makingReservingPersonField = value;
        }

        [XmlElement("ApplicableTravelPayment")]
        public TravelPaymentType[] ApplicableTravelPayment
        {
            get => this.applicableTravelPaymentField;
            set => this.applicableTravelPaymentField = value;
        }
    }
}