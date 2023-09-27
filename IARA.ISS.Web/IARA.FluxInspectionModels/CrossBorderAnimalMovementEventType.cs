namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CrossBorderAnimalMovementEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CrossBorderAnimalMovementEventType
    {

        private DateTimeType importOccurrenceDateTimeField;

        private DateTimeType exportOccurrenceDateTimeField;

        private IDType departureLocationIDField;

        private IDType arrivalLocationIDField;

        private ReferencedCountryType originRelatedReferencedCountryField;

        private ReferencedCountryType destinationRelatedReferencedCountryField;

        public DateTimeType ImportOccurrenceDateTime
        {
            get => this.importOccurrenceDateTimeField;
            set => this.importOccurrenceDateTimeField = value;
        }

        public DateTimeType ExportOccurrenceDateTime
        {
            get => this.exportOccurrenceDateTimeField;
            set => this.exportOccurrenceDateTimeField = value;
        }

        public IDType DepartureLocationID
        {
            get => this.departureLocationIDField;
            set => this.departureLocationIDField = value;
        }

        public IDType ArrivalLocationID
        {
            get => this.arrivalLocationIDField;
            set => this.arrivalLocationIDField = value;
        }

        public ReferencedCountryType OriginRelatedReferencedCountry
        {
            get => this.originRelatedReferencedCountryField;
            set => this.originRelatedReferencedCountryField = value;
        }

        public ReferencedCountryType DestinationRelatedReferencedCountry
        {
            get => this.destinationRelatedReferencedCountryField;
            set => this.destinationRelatedReferencedCountryField = value;
        }
    }
}