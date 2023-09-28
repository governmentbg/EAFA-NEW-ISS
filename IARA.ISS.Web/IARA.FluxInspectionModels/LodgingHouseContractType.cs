namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseContract", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseContractType
    {

        private IDType idField;

        private IDType tradingPartnerIDField;

        private DateType startDateField;

        private IndicatorType automaticExtensionIndicatorField;

        private SpecifiedPeriodType effectiveSpecifiedPeriodField;

        private LodgingHousePartyType identifiedLodgingHousePartyField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType TradingPartnerID
        {
            get => this.tradingPartnerIDField;
            set => this.tradingPartnerIDField = value;
        }

        public DateType StartDate
        {
            get => this.startDateField;
            set => this.startDateField = value;
        }

        public IndicatorType AutomaticExtensionIndicator
        {
            get => this.automaticExtensionIndicatorField;
            set => this.automaticExtensionIndicatorField = value;
        }

        public SpecifiedPeriodType EffectiveSpecifiedPeriod
        {
            get => this.effectiveSpecifiedPeriodField;
            set => this.effectiveSpecifiedPeriodField = value;
        }

        public LodgingHousePartyType IdentifiedLodgingHouseParty
        {
            get => this.identifiedLodgingHousePartyField;
            set => this.identifiedLodgingHousePartyField = value;
        }
    }
}