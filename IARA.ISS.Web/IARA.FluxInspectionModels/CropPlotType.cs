namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CropPlot", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CropPlotType
    {

        private IDType idField;

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

        private MeasureType areaMeasureField;

        private CodeType regulatorySoilTypeCodeField;

        private IndicatorType regulatoryOrganicIndicatorField;

        private AgriculturalCharacteristicType[] specifiedAgriculturalCharacteristicField;

        private AgriculturalZoneAreaType[] specifiedAgriculturalZoneAreaField;

        private CropPlotType[] includedCropPlotField;

        private ReferencedLocationType specifiedReferencedLocationField;

        private MobilePlotType includedMobilePlotField;

        private AgriculturalCertificateType[] specifiedAgriculturalCertificateField;

        private FieldCropType[] grownFieldCropField;

        private CropProductionAgriculturalProcessType[] applicableCropProductionAgriculturalProcessField;

        private SpecifiedAgriculturalApplicationType[] appliedSpecifiedAgriculturalApplicationField;

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

        public MeasureType AreaMeasure
        {
            get
            {
                return this.areaMeasureField;
            }
            set
            {
                this.areaMeasureField = value;
            }
        }

        public CodeType RegulatorySoilTypeCode
        {
            get
            {
                return this.regulatorySoilTypeCodeField;
            }
            set
            {
                this.regulatorySoilTypeCodeField = value;
            }
        }

        public IndicatorType RegulatoryOrganicIndicator
        {
            get
            {
                return this.regulatoryOrganicIndicatorField;
            }
            set
            {
                this.regulatoryOrganicIndicatorField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalCharacteristic")]
        public AgriculturalCharacteristicType[] SpecifiedAgriculturalCharacteristic
        {
            get
            {
                return this.specifiedAgriculturalCharacteristicField;
            }
            set
            {
                this.specifiedAgriculturalCharacteristicField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalZoneArea")]
        public AgriculturalZoneAreaType[] SpecifiedAgriculturalZoneArea
        {
            get
            {
                return this.specifiedAgriculturalZoneAreaField;
            }
            set
            {
                this.specifiedAgriculturalZoneAreaField = value;
            }
        }

        [XmlElement("IncludedCropPlot")]
        public CropPlotType[] IncludedCropPlot
        {
            get
            {
                return this.includedCropPlotField;
            }
            set
            {
                this.includedCropPlotField = value;
            }
        }

        public ReferencedLocationType SpecifiedReferencedLocation
        {
            get
            {
                return this.specifiedReferencedLocationField;
            }
            set
            {
                this.specifiedReferencedLocationField = value;
            }
        }

        public MobilePlotType IncludedMobilePlot
        {
            get
            {
                return this.includedMobilePlotField;
            }
            set
            {
                this.includedMobilePlotField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalCertificate")]
        public AgriculturalCertificateType[] SpecifiedAgriculturalCertificate
        {
            get
            {
                return this.specifiedAgriculturalCertificateField;
            }
            set
            {
                this.specifiedAgriculturalCertificateField = value;
            }
        }

        [XmlElement("GrownFieldCrop")]
        public FieldCropType[] GrownFieldCrop
        {
            get
            {
                return this.grownFieldCropField;
            }
            set
            {
                this.grownFieldCropField = value;
            }
        }

        [XmlElement("ApplicableCropProductionAgriculturalProcess")]
        public CropProductionAgriculturalProcessType[] ApplicableCropProductionAgriculturalProcess
        {
            get
            {
                return this.applicableCropProductionAgriculturalProcessField;
            }
            set
            {
                this.applicableCropProductionAgriculturalProcessField = value;
            }
        }

        [XmlElement("AppliedSpecifiedAgriculturalApplication")]
        public SpecifiedAgriculturalApplicationType[] AppliedSpecifiedAgriculturalApplication
        {
            get
            {
                return this.appliedSpecifiedAgriculturalApplicationField;
            }
            set
            {
                this.appliedSpecifiedAgriculturalApplicationField = value;
            }
        }
    }
}