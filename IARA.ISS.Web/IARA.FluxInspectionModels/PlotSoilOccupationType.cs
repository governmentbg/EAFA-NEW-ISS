namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PlotSoilOccupation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PlotSoilOccupationType
    {

        private CodeType typeCodeField;

        private NumericType sequenceNumericField;

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

        private AgriculturalCropType[] sownAgriculturalCropField;

        private SoilOccupationCropResidueType[] previousSoilOccupationCropResidueField;

        private AgriculturalCountrySubDivisionType[] occurrenceAgriculturalCountrySubDivisionField;

        private AgriculturalCropProductionCycleType[] specifiedAgriculturalCropProductionCycleField;

        private SoilOccupationContractType[] applicableSoilOccupationContractField;

        private AgriculturalAreaType[] usedAgriculturalAreaField;

        private SpecifiedSoilSupplementType[] appliedSpecifiedSoilSupplementField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

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

        [XmlElement("SownAgriculturalCrop")]
        public AgriculturalCropType[] SownAgriculturalCrop
        {
            get
            {
                return this.sownAgriculturalCropField;
            }
            set
            {
                this.sownAgriculturalCropField = value;
            }
        }

        [XmlElement("PreviousSoilOccupationCropResidue")]
        public SoilOccupationCropResidueType[] PreviousSoilOccupationCropResidue
        {
            get
            {
                return this.previousSoilOccupationCropResidueField;
            }
            set
            {
                this.previousSoilOccupationCropResidueField = value;
            }
        }

        [XmlElement("OccurrenceAgriculturalCountrySubDivision")]
        public AgriculturalCountrySubDivisionType[] OccurrenceAgriculturalCountrySubDivision
        {
            get
            {
                return this.occurrenceAgriculturalCountrySubDivisionField;
            }
            set
            {
                this.occurrenceAgriculturalCountrySubDivisionField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalCropProductionCycle")]
        public AgriculturalCropProductionCycleType[] SpecifiedAgriculturalCropProductionCycle
        {
            get
            {
                return this.specifiedAgriculturalCropProductionCycleField;
            }
            set
            {
                this.specifiedAgriculturalCropProductionCycleField = value;
            }
        }

        [XmlElement("ApplicableSoilOccupationContract")]
        public SoilOccupationContractType[] ApplicableSoilOccupationContract
        {
            get
            {
                return this.applicableSoilOccupationContractField;
            }
            set
            {
                this.applicableSoilOccupationContractField = value;
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

        [XmlElement("AppliedSpecifiedSoilSupplement")]
        public SpecifiedSoilSupplementType[] AppliedSpecifiedSoilSupplement
        {
            get
            {
                return this.appliedSpecifiedSoilSupplementField;
            }
            set
            {
                this.appliedSpecifiedSoilSupplementField = value;
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
    }
}