namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalPlot", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalPlotType
    {

        private IDType idField;

        private IDType previousIDField;

        private TextType descriptionField;

        private DateTimeType dataSheetStartDateTimeField;

        private DateTimeType dataSheetEndDateTimeField;

        private NumericType sequenceNumericField;

        private IndicatorType dividedIndicatorField;

        private IDType issuerInternalIDField;

        private IDType[] recipientInternalIDField;

        private DateTimeType harvestYearDateTimeField;

        private IDType cAPIDField;

        private AgriculturalCountrySubDivisionType[] includedInAgriculturalCountrySubDivisionField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        private PlotSoilOccupationType[] appliedPlotSoilOccupationField;

        private AgriculturalAreaType[] includedAgriculturalAreaField;

        private AgriculturalAnalysisType[] soilAgriculturalAnalysisField;

        private AgronomicalObservationPartyType[] ownerAgronomicalObservationPartyField;

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

        public IDType PreviousID
        {
            get
            {
                return this.previousIDField;
            }
            set
            {
                this.previousIDField = value;
            }
        }

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public DateTimeType DataSheetStartDateTime
        {
            get
            {
                return this.dataSheetStartDateTimeField;
            }
            set
            {
                this.dataSheetStartDateTimeField = value;
            }
        }

        public DateTimeType DataSheetEndDateTime
        {
            get
            {
                return this.dataSheetEndDateTimeField;
            }
            set
            {
                this.dataSheetEndDateTimeField = value;
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

        public IndicatorType DividedIndicator
        {
            get
            {
                return this.dividedIndicatorField;
            }
            set
            {
                this.dividedIndicatorField = value;
            }
        }

        public IDType IssuerInternalID
        {
            get
            {
                return this.issuerInternalIDField;
            }
            set
            {
                this.issuerInternalIDField = value;
            }
        }

        [XmlElement("RecipientInternalID")]
        public IDType[] RecipientInternalID
        {
            get
            {
                return this.recipientInternalIDField;
            }
            set
            {
                this.recipientInternalIDField = value;
            }
        }

        public DateTimeType HarvestYearDateTime
        {
            get
            {
                return this.harvestYearDateTimeField;
            }
            set
            {
                this.harvestYearDateTimeField = value;
            }
        }

        public IDType CAPID
        {
            get
            {
                return this.cAPIDField;
            }
            set
            {
                this.cAPIDField = value;
            }
        }

        [XmlElement("IncludedInAgriculturalCountrySubDivision")]
        public AgriculturalCountrySubDivisionType[] IncludedInAgriculturalCountrySubDivision
        {
            get
            {
                return this.includedInAgriculturalCountrySubDivisionField;
            }
            set
            {
                this.includedInAgriculturalCountrySubDivisionField = value;
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

        [XmlElement("AppliedPlotSoilOccupation")]
        public PlotSoilOccupationType[] AppliedPlotSoilOccupation
        {
            get
            {
                return this.appliedPlotSoilOccupationField;
            }
            set
            {
                this.appliedPlotSoilOccupationField = value;
            }
        }

        [XmlElement("IncludedAgriculturalArea")]
        public AgriculturalAreaType[] IncludedAgriculturalArea
        {
            get
            {
                return this.includedAgriculturalAreaField;
            }
            set
            {
                this.includedAgriculturalAreaField = value;
            }
        }

        [XmlElement("SoilAgriculturalAnalysis")]
        public AgriculturalAnalysisType[] SoilAgriculturalAnalysis
        {
            get
            {
                return this.soilAgriculturalAnalysisField;
            }
            set
            {
                this.soilAgriculturalAnalysisField = value;
            }
        }

        [XmlElement("OwnerAgronomicalObservationParty")]
        public AgronomicalObservationPartyType[] OwnerAgronomicalObservationParty
        {
            get
            {
                return this.ownerAgronomicalObservationPartyField;
            }
            set
            {
                this.ownerAgronomicalObservationPartyField = value;
            }
        }
    }
}