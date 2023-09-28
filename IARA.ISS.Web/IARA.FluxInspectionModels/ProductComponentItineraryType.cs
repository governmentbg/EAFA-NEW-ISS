namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProductComponentItinerary", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProductComponentItineraryType
    {

        private NumericType dayNumericField;

        private TimeType productStartTimeField;

        private TimeType productEndTimeField;

        private TextType specifiedAreaField;

        private LodgingHouseTravelProductSubComponentType includedLodgingHouseTravelProductSubComponentField;

        public NumericType DayNumeric
        {
            get => this.dayNumericField;
            set => this.dayNumericField = value;
        }

        public TimeType ProductStartTime
        {
            get => this.productStartTimeField;
            set => this.productStartTimeField = value;
        }

        public TimeType ProductEndTime
        {
            get => this.productEndTimeField;
            set => this.productEndTimeField = value;
        }

        public TextType SpecifiedArea
        {
            get => this.specifiedAreaField;
            set => this.specifiedAreaField = value;
        }

        public LodgingHouseTravelProductSubComponentType IncludedLodgingHouseTravelProductSubComponent
        {
            get => this.includedLodgingHouseTravelProductSubComponentField;
            set => this.includedLodgingHouseTravelProductSubComponentField = value;
        }
    }
}