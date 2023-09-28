namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAPProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAPProductType
    {

        private CodeType speciesCodeField;

        private QuantityType unitQuantityField;

        private MeasureType weightMeasureField;

        private CodeType weighingMeansCodeField;

        private CodeType usageCodeField;

        private QuantityType packagingUnitQuantityField;

        private CodeType packagingTypeCodeField;

        private MeasureType packagingUnitAverageWeightMeasureField;

        private AAPProcessType[] appliedAAPProcessField;

        private AmountType[] totalSalesPriceField;

        private SizeDistributionType specifiedSizeDistributionField;

        private FLUXLocationType[] originFLUXLocationField;

        private FishingActivityType originFishingActivityField;

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

        public CodeType WeighingMeansCode
        {
            get
            {
                return this.weighingMeansCodeField;
            }
            set
            {
                this.weighingMeansCodeField = value;
            }
        }

        public CodeType UsageCode
        {
            get
            {
                return this.usageCodeField;
            }
            set
            {
                this.usageCodeField = value;
            }
        }

        public QuantityType PackagingUnitQuantity
        {
            get
            {
                return this.packagingUnitQuantityField;
            }
            set
            {
                this.packagingUnitQuantityField = value;
            }
        }

        public CodeType PackagingTypeCode
        {
            get
            {
                return this.packagingTypeCodeField;
            }
            set
            {
                this.packagingTypeCodeField = value;
            }
        }

        public MeasureType PackagingUnitAverageWeightMeasure
        {
            get
            {
                return this.packagingUnitAverageWeightMeasureField;
            }
            set
            {
                this.packagingUnitAverageWeightMeasureField = value;
            }
        }

        [XmlElement("AppliedAAPProcess")]
        public AAPProcessType[] AppliedAAPProcess
        {
            get
            {
                return this.appliedAAPProcessField;
            }
            set
            {
                this.appliedAAPProcessField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("ChargeAmount", IsNullable = false)]
        public AmountType[] TotalSalesPrice
        {
            get
            {
                return this.totalSalesPriceField;
            }
            set
            {
                this.totalSalesPriceField = value;
            }
        }

        public SizeDistributionType SpecifiedSizeDistribution
        {
            get
            {
                return this.specifiedSizeDistributionField;
            }
            set
            {
                this.specifiedSizeDistributionField = value;
            }
        }

        [XmlElement("OriginFLUXLocation")]
        public FLUXLocationType[] OriginFLUXLocation
        {
            get
            {
                return this.originFLUXLocationField;
            }
            set
            {
                this.originFLUXLocationField = value;
            }
        }

        public FishingActivityType OriginFishingActivity
        {
            get
            {
                return this.originFishingActivityField;
            }
            set
            {
                this.originFishingActivityField = value;
            }
        }
    }
}