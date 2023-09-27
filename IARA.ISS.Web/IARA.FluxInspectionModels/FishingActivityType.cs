namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FishingActivity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FishingActivityType
    {

        private IDType[] idField;

        private CodeType typeCodeField;

        private DateTimeType occurrenceDateTimeField;

        private CodeType reasonCodeField;

        private CodeType vesselRelatedActivityCodeField;

        private CodeType fisheryTypeCodeField;

        private CodeType speciesTargetCodeField;

        private QuantityType operationsQuantityField;

        private MeasureType fishingDurationMeasureField;

        private FACatchType[] specifiedFACatchField;

        private FLUXLocationType[] relatedFLUXLocationField;

        private GearProblemType[] specifiedGearProblemField;

        private FLUXCharacteristicType[] specifiedFLUXCharacteristicField;

        private FishingGearType[] specifiedFishingGearField;

        private VesselStorageCharacteristicType sourceVesselStorageCharacteristicField;

        private VesselStorageCharacteristicType destinationVesselStorageCharacteristicField;

        private FishingActivityType[] relatedFishingActivityField;

        private FLAPDocumentType[] specifiedFLAPDocumentField;

        private DelimitedPeriodType[] specifiedDelimitedPeriodField;

        private FishingTripType specifiedFishingTripField;

        private VesselTransportMeansType[] relatedVesselTransportMeansField;

        [XmlElement("ID")]
        public IDType[] ID
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

        public DateTimeType OccurrenceDateTime
        {
            get
            {
                return this.occurrenceDateTimeField;
            }
            set
            {
                this.occurrenceDateTimeField = value;
            }
        }

        public CodeType ReasonCode
        {
            get
            {
                return this.reasonCodeField;
            }
            set
            {
                this.reasonCodeField = value;
            }
        }

        public CodeType VesselRelatedActivityCode
        {
            get
            {
                return this.vesselRelatedActivityCodeField;
            }
            set
            {
                this.vesselRelatedActivityCodeField = value;
            }
        }

        public CodeType FisheryTypeCode
        {
            get
            {
                return this.fisheryTypeCodeField;
            }
            set
            {
                this.fisheryTypeCodeField = value;
            }
        }

        public CodeType SpeciesTargetCode
        {
            get
            {
                return this.speciesTargetCodeField;
            }
            set
            {
                this.speciesTargetCodeField = value;
            }
        }

        public QuantityType OperationsQuantity
        {
            get
            {
                return this.operationsQuantityField;
            }
            set
            {
                this.operationsQuantityField = value;
            }
        }

        public MeasureType FishingDurationMeasure
        {
            get
            {
                return this.fishingDurationMeasureField;
            }
            set
            {
                this.fishingDurationMeasureField = value;
            }
        }

        [XmlElement("SpecifiedFACatch")]
        public FACatchType[] SpecifiedFACatch
        {
            get
            {
                return this.specifiedFACatchField;
            }
            set
            {
                this.specifiedFACatchField = value;
            }
        }

        [XmlElement("RelatedFLUXLocation")]
        public FLUXLocationType[] RelatedFLUXLocation
        {
            get
            {
                return this.relatedFLUXLocationField;
            }
            set
            {
                this.relatedFLUXLocationField = value;
            }
        }

        [XmlElement("SpecifiedGearProblem")]
        public GearProblemType[] SpecifiedGearProblem
        {
            get
            {
                return this.specifiedGearProblemField;
            }
            set
            {
                this.specifiedGearProblemField = value;
            }
        }

        [XmlElement("SpecifiedFLUXCharacteristic")]
        public FLUXCharacteristicType[] SpecifiedFLUXCharacteristic
        {
            get
            {
                return this.specifiedFLUXCharacteristicField;
            }
            set
            {
                this.specifiedFLUXCharacteristicField = value;
            }
        }

        [XmlElement("SpecifiedFishingGear")]
        public FishingGearType[] SpecifiedFishingGear
        {
            get
            {
                return this.specifiedFishingGearField;
            }
            set
            {
                this.specifiedFishingGearField = value;
            }
        }

        public VesselStorageCharacteristicType SourceVesselStorageCharacteristic
        {
            get
            {
                return this.sourceVesselStorageCharacteristicField;
            }
            set
            {
                this.sourceVesselStorageCharacteristicField = value;
            }
        }

        public VesselStorageCharacteristicType DestinationVesselStorageCharacteristic
        {
            get
            {
                return this.destinationVesselStorageCharacteristicField;
            }
            set
            {
                this.destinationVesselStorageCharacteristicField = value;
            }
        }

        [XmlElement("RelatedFishingActivity")]
        public FishingActivityType[] RelatedFishingActivity
        {
            get
            {
                return this.relatedFishingActivityField;
            }
            set
            {
                this.relatedFishingActivityField = value;
            }
        }

        [XmlElement("SpecifiedFLAPDocument")]
        public FLAPDocumentType[] SpecifiedFLAPDocument
        {
            get
            {
                return this.specifiedFLAPDocumentField;
            }
            set
            {
                this.specifiedFLAPDocumentField = value;
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

        public FishingTripType SpecifiedFishingTrip
        {
            get
            {
                return this.specifiedFishingTripField;
            }
            set
            {
                this.specifiedFishingTripField = value;
            }
        }

        [XmlElement("RelatedVesselTransportMeans")]
        public VesselTransportMeansType[] RelatedVesselTransportMeans
        {
            get
            {
                return this.relatedVesselTransportMeansField;
            }
            set
            {
                this.relatedVesselTransportMeansField = value;
            }
        }
    }
}