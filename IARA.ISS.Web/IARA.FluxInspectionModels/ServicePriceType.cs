namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ServicePrice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ServicePriceType
    {

        private CodeType seasonalRankCodeField;

        private QuantityType basisQuantityField;

        private IndicatorType netPriceIndicatorField;

        private AmountType chargeAmountField;

        private PercentType calculationPercentField;

        private IndicatorType taxChargeApplicableIndicatorField;

        private IndicatorType commissionPaidIndicatorField;

        private TravelProductPeriodType[] validityTravelProductPeriodField;

        private SeasonalPeriodType[] validitySeasonalPeriodField;

        private TravelProductNoteType[] additionalInformationTravelProductNoteField;

        private LodgingHouseCustomerClassType[] applicableLodgingHouseCustomerClassField;

        public CodeType SeasonalRankCode
        {
            get => this.seasonalRankCodeField;
            set => this.seasonalRankCodeField = value;
        }

        public QuantityType BasisQuantity
        {
            get => this.basisQuantityField;
            set => this.basisQuantityField = value;
        }

        public IndicatorType NetPriceIndicator
        {
            get => this.netPriceIndicatorField;
            set => this.netPriceIndicatorField = value;
        }

        public AmountType ChargeAmount
        {
            get => this.chargeAmountField;
            set => this.chargeAmountField = value;
        }

        public PercentType CalculationPercent
        {
            get => this.calculationPercentField;
            set => this.calculationPercentField = value;
        }

        public IndicatorType TaxChargeApplicableIndicator
        {
            get => this.taxChargeApplicableIndicatorField;
            set => this.taxChargeApplicableIndicatorField = value;
        }

        public IndicatorType CommissionPaidIndicator
        {
            get => this.commissionPaidIndicatorField;
            set => this.commissionPaidIndicatorField = value;
        }

        [XmlElement("ValidityTravelProductPeriod")]
        public TravelProductPeriodType[] ValidityTravelProductPeriod
        {
            get => this.validityTravelProductPeriodField;
            set => this.validityTravelProductPeriodField = value;
        }

        [XmlElement("ValiditySeasonalPeriod")]
        public SeasonalPeriodType[] ValiditySeasonalPeriod
        {
            get => this.validitySeasonalPeriodField;
            set => this.validitySeasonalPeriodField = value;
        }

        [XmlElement("AdditionalInformationTravelProductNote")]
        public TravelProductNoteType[] AdditionalInformationTravelProductNote
        {
            get => this.additionalInformationTravelProductNoteField;
            set => this.additionalInformationTravelProductNoteField = value;
        }

        [XmlElement("ApplicableLodgingHouseCustomerClass")]
        public LodgingHouseCustomerClassType[] ApplicableLodgingHouseCustomerClass
        {
            get => this.applicableLodgingHouseCustomerClassField;
            set => this.applicableLodgingHouseCustomerClassField = value;
        }
    }
}