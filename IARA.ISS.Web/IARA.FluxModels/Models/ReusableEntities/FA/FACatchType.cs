using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
[Serializable]


    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20" +
        "")]
    public partial class FACatchType
    {

        private CodeType speciesCodeField;

        private QuantityType unitQuantityField;

        private MeasureType weightMeasureField;

        private CodeType weighingMeansCodeField;

        private CodeType usageCodeField;

        private CodeType typeCodeField;

        private NumericType toleranceMarginNumericField;

        private FishingTripType[] relatedFishingTripField;

        private SizeDistributionType specifiedSizeDistributionField;

        private AAPStockType[] relatedAAPStockField;

        private AAPProcessType[] appliedAAPProcessField;

        private SalesBatchType[] relatedSalesBatchField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        private FishingGearType[] usedFishingGearField;

        private FLUXCharacteristicType[] applicableFLUXCharacteristicField;

        private FLUXLocationType[] destinationFLUXLocationField;

        private DelimitedPeriodType[] specifiedDelimitedPeriodField;


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


        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }


        public NumericType ToleranceMarginNumeric
        {
            get
            {
                return this.toleranceMarginNumericField;
            }
            set
            {
                this.toleranceMarginNumericField = value;
            }
        }


        [XmlElement("RelatedFishingTrip")]
        public FishingTripType[] RelatedFishingTrip
        {
            get
            {
                return this.relatedFishingTripField;
            }
            set
            {
                this.relatedFishingTripField = value;
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


        [XmlElement("RelatedAAPStock")]
        public AAPStockType[] RelatedAAPStock
        {
            get
            {
                return this.relatedAAPStockField;
            }
            set
            {
                this.relatedAAPStockField = value;
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


        [XmlElement("RelatedSalesBatch")]
        public SalesBatchType[] RelatedSalesBatch
        {
            get
            {
                return this.relatedSalesBatchField;
            }
            set
            {
                this.relatedSalesBatchField = value;
            }
        }


        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation
        {
            get
            {
                return this.specifiedFLUXLocationField;
            }
            set
            {
                this.specifiedFLUXLocationField = value;
            }
        }


        [XmlElement("UsedFishingGear")]
        public FishingGearType[] UsedFishingGear
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


        [XmlElement("ApplicableFLUXCharacteristic")]
        public FLUXCharacteristicType[] ApplicableFLUXCharacteristic
        {
            get
            {
                return this.applicableFLUXCharacteristicField;
            }
            set
            {
                this.applicableFLUXCharacteristicField = value;
            }
        }


        [XmlElement("DestinationFLUXLocation")]
        public FLUXLocationType[] DestinationFLUXLocation
        {
            get
            {
                return this.destinationFLUXLocationField;
            }
            set
            {
                this.destinationFLUXLocationField = value;
            }
        }


        [XmlElement("SpecifiedDelimitedPeriod")]
        public DelimitedPeriodType[] SpecifiedDelimitedPeriod
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
    }
}