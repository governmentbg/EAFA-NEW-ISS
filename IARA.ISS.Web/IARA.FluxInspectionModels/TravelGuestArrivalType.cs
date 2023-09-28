namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TravelGuestArrival", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TravelGuestArrivalType
    {

        private CodeType transportModeCodeField;

        private TextType carrierNameField;

        private IDType carrierIDField;

        private DateTimeType expectedDateTimeField;

        private TextType descriptionField;

        public CodeType TransportModeCode
        {
            get => this.transportModeCodeField;
            set => this.transportModeCodeField = value;
        }

        public TextType CarrierName
        {
            get => this.carrierNameField;
            set => this.carrierNameField = value;
        }

        public IDType CarrierID
        {
            get => this.carrierIDField;
            set => this.carrierIDField = value;
        }

        public DateTimeType ExpectedDateTime
        {
            get => this.expectedDateTimeField;
            set => this.expectedDateTimeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}