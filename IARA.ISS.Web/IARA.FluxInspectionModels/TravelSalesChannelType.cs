namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TravelSalesChannel", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TravelSalesChannelType
    {

        private CodeType categoryCodeField;

        private DateTimeType reservationRequestedDateTimeField;

        private IDType reservationDataLocatorIDField;

        private CodeType statusCodeField;

        private TextType descriptionField;

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public DateTimeType ReservationRequestedDateTime
        {
            get => this.reservationRequestedDateTimeField;
            set => this.reservationRequestedDateTimeField = value;
        }

        public IDType ReservationDataLocatorID
        {
            get => this.reservationDataLocatorIDField;
            set => this.reservationDataLocatorIDField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}