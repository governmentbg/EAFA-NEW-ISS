namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalCropProductionCycle", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalCropProductionCycleType
    {

        private NumericType sequenceNumericField;

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

        private DateTimeType productionYearDateTimeField;

        private PlotAgriculturalProcessType[] applicablePlotAgriculturalProcessField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        private AgriculturalAreaType[] usedAgriculturalAreaField;

        public NumericType SequenceNumeric
        {
            get
            {
                return this.sequenceNumericField;
            }
            set
            {
                this.sequenceNumericField = value;
            }
        }

        public DateTimeType StartDateTime
        {
            get
            {
                return this.startDateTimeField;
            }
            set
            {
                this.startDateTimeField = value;
            }
        }

        public DateTimeType EndDateTime
        {
            get
            {
                return this.endDateTimeField;
            }
            set
            {
                this.endDateTimeField = value;
            }
        }

        public DateTimeType ProductionYearDateTime
        {
            get
            {
                return this.productionYearDateTimeField;
            }
            set
            {
                this.productionYearDateTimeField = value;
            }
        }

        [XmlElement("ApplicablePlotAgriculturalProcess")]
        public PlotAgriculturalProcessType[] ApplicablePlotAgriculturalProcess
        {
            get
            {
                return this.applicablePlotAgriculturalProcessField;
            }
            set
            {
                this.applicablePlotAgriculturalProcessField = value;
            }
        }

        [XmlElement("ApplicableTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ApplicableTechnicalCharacteristic
        {
            get
            {
                return this.applicableTechnicalCharacteristicField;
            }
            set
            {
                this.applicableTechnicalCharacteristicField = value;
            }
        }

        [XmlElement("UsedAgriculturalArea")]
        public AgriculturalAreaType[] UsedAgriculturalArea
        {
            get
            {
                return this.usedAgriculturalAreaField;
            }
            set
            {
                this.usedAgriculturalAreaField = value;
            }
        }
    }
}