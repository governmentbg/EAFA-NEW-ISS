namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("BatchCatch", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class BatchCatchType
    {

        private IDType idField;

        private CodeType speciesCodeField;

        private QuantityType unitQuantityField;

        private MeasureType weightMeasureField;

        private TTTransportMeansType specifiedTTTransportMeansField;

        private DelimitedPeriodType specifiedDelimitedPeriodField;

        private TTLocationType[] specifiedTTLocationField;

        private FishingGearType usedFishingGearField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public CodeType SpeciesCode
        {
            get
            {
                return this.speciesCodeField;
            }
            set
            {
                this.speciesCodeField = value;
            }
        }

        public QuantityType UnitQuantity
        {
            get
            {
                return this.unitQuantityField;
            }
            set
            {
                this.unitQuantityField = value;
            }
        }

        public MeasureType WeightMeasure
        {
            get
            {
                return this.weightMeasureField;
            }
            set
            {
                this.weightMeasureField = value;
            }
        }

        public TTTransportMeansType SpecifiedTTTransportMeans
        {
            get
            {
                return this.specifiedTTTransportMeansField;
            }
            set
            {
                this.specifiedTTTransportMeansField = value;
            }
        }

        public DelimitedPeriodType SpecifiedDelimitedPeriod
        {
            get
            {
                return this.specifiedDelimitedPeriodField;
            }
            set
            {
                this.specifiedDelimitedPeriodField = value;
            }
        }

        [XmlElement("SpecifiedTTLocation")]
        public TTLocationType[] SpecifiedTTLocation
        {
            get
            {
                return this.specifiedTTLocationField;
            }
            set
            {
                this.specifiedTTLocationField = value;
            }
        }

        public FishingGearType UsedFishingGear
        {
            get
            {
                return this.usedFishingGearField;
            }
            set
            {
                this.usedFishingGearField = value;
            }
        }
    }
}