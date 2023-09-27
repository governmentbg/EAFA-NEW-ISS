namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CancellationPrice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CancellationPriceType
    {

        private CodeType seasonalRankCodeField;

        private CodeType determinationCodeField;

        private QuantityType basisQuantityField;

        private IndicatorType netPriceIndicatorField;

        private AmountType chargeAmountField;

        private PercentType calculationPercentField;

        private TravelProductPeriodType[] validityTravelProductPeriodField;

        private SeasonalPeriodType[] validitySeasonalPeriodField;

        private TravelProductNoteType[] additionalInformationTravelProductNoteField;

        private LodgingHouseCustomerClassType[] applicableLodgingHouseCustomerClassField;

        public CodeType SeasonalRankCode
        {
            get
            {
                return this.seasonalRankCodeField;
            }
            set
            {
                this.seasonalRankCodeField = value;
            }
        }

        public CodeType DeterminationCode
        {
            get
            {
                return this.determinationCodeField;
            }
            set
            {
                this.determinationCodeField = value;
            }
        }

        public QuantityType BasisQuantity
        {
            get
            {
                return this.basisQuantityField;
            }
            set
            {
                this.basisQuantityField = value;
            }
        }

        public IndicatorType NetPriceIndicator
        {
            get
            {
                return this.netPriceIndicatorField;
            }
            set
            {
                this.netPriceIndicatorField = value;
            }
        }

        public AmountType ChargeAmount
        {
            get
            {
                return this.chargeAmountField;
            }
            set
            {
                this.chargeAmountField = value;
            }
        }

        public PercentType CalculationPercent
        {
            get
            {
                return this.calculationPercentField;
            }
            set
            {
                this.calculationPercentField = value;
            }
        }

        [XmlElement("ValidityTravelProductPeriod")]
        public TravelProductPeriodType[] ValidityTravelProductPeriod
        {
            get
            {
                return this.validityTravelProductPeriodField;
            }
            set
            {
                this.validityTravelProductPeriodField = value;
            }
        }

        [XmlElement("ValiditySeasonalPeriod")]
        public SeasonalPeriodType[] ValiditySeasonalPeriod
        {
            get
            {
                return this.validitySeasonalPeriodField;
            }
            set
            {
                this.validitySeasonalPeriodField = value;
            }
        }

        [XmlElement("AdditionalInformationTravelProductNote")]
        public TravelProductNoteType[] AdditionalInformationTravelProductNote
        {
            get
            {
                return this.additionalInformationTravelProductNoteField;
            }
            set
            {
                this.additionalInformationTravelProductNoteField = value;
            }
        }

        [XmlElement("ApplicableLodgingHouseCustomerClass")]
        public LodgingHouseCustomerClassType[] ApplicableLodgingHouseCustomerClass
        {
            get
            {
                return this.applicableLodgingHouseCustomerClassField;
            }
            set
            {
                this.applicableLodgingHouseCustomerClassField = value;
            }
        }
    }
}