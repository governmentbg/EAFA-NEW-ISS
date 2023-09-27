namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FieldCrop", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FieldCropType
    {

        private CodeType sowingPeriodCodeField;

        private CodeType[] purposeCodeField;

        private CodeType[] plantingReasonCodeField;

        private TextType descriptionField;

        private CodeType[] classificationCodeField;

        private CodeType cultivationTypeCodeField;

        private IndicatorType propagationMaterialIndicatorField;

        private CodeType[] cultivationCoverageCodeField;

        private CodeType cultivationMediumCodeField;

        private CodeType cultivationContainerCodeField;

        private DateTimeType harvestDateTimeField;

        private TextType classNameField;

        private CodeType productionPeriodCodeField;

        private CodeType productionEnvironmentCodeField;

        private FieldCropType[] grownPreviousFieldCropField;

        private CropPlotType grownCropPlotField;

        private FieldCropMixtureConstituentType[] specifiedFieldCropMixtureConstituentField;

        private CropProduceType[] harvestedCropProduceField;

        private ReferencedObservationResultType[] specifiedReferencedObservationResultField;

        private CropProductionAgriculturalProcessType[] applicableCropProductionAgriculturalProcessField;

        private SingleAgronomicalObservationType[] specifiedSingleAgronomicalObservationField;

        private AgriculturalCharacteristicType[] specifiedAgriculturalCharacteristicField;

        private SpecifiedAgriculturalApplicationType[] appliedSpecifiedAgriculturalApplicationField;

        private AgriculturalExtremeWeatherConditionType[] specifiedAgriculturalExtremeWeatherConditionField;

        public CodeType SowingPeriodCode
        {
            get
            {
                return this.sowingPeriodCodeField;
            }
            set
            {
                this.sowingPeriodCodeField = value;
            }
        }

        [XmlElement("PurposeCode")]
        public CodeType[] PurposeCode
        {
            get
            {
                return this.purposeCodeField;
            }
            set
            {
                this.purposeCodeField = value;
            }
        }

        [XmlElement("PlantingReasonCode")]
        public CodeType[] PlantingReasonCode
        {
            get
            {
                return this.plantingReasonCodeField;
            }
            set
            {
                this.plantingReasonCodeField = value;
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

        [XmlElement("ClassificationCode")]
        public CodeType[] ClassificationCode
        {
            get
            {
                return this.classificationCodeField;
            }
            set
            {
                this.classificationCodeField = value;
            }
        }

        public CodeType CultivationTypeCode
        {
            get
            {
                return this.cultivationTypeCodeField;
            }
            set
            {
                this.cultivationTypeCodeField = value;
            }
        }

        public IndicatorType PropagationMaterialIndicator
        {
            get
            {
                return this.propagationMaterialIndicatorField;
            }
            set
            {
                this.propagationMaterialIndicatorField = value;
            }
        }

        [XmlElement("CultivationCoverageCode")]
        public CodeType[] CultivationCoverageCode
        {
            get
            {
                return this.cultivationCoverageCodeField;
            }
            set
            {
                this.cultivationCoverageCodeField = value;
            }
        }

        public CodeType CultivationMediumCode
        {
            get
            {
                return this.cultivationMediumCodeField;
            }
            set
            {
                this.cultivationMediumCodeField = value;
            }
        }

        public CodeType CultivationContainerCode
        {
            get
            {
                return this.cultivationContainerCodeField;
            }
            set
            {
                this.cultivationContainerCodeField = value;
            }
        }

        public DateTimeType HarvestDateTime
        {
            get
            {
                return this.harvestDateTimeField;
            }
            set
            {
                this.harvestDateTimeField = value;
            }
        }

        public TextType ClassName
        {
            get
            {
                return this.classNameField;
            }
            set
            {
                this.classNameField = value;
            }
        }

        public CodeType ProductionPeriodCode
        {
            get
            {
                return this.productionPeriodCodeField;
            }
            set
            {
                this.productionPeriodCodeField = value;
            }
        }

        public CodeType ProductionEnvironmentCode
        {
            get
            {
                return this.productionEnvironmentCodeField;
            }
            set
            {
                this.productionEnvironmentCodeField = value;
            }
        }

        [XmlElement("GrownPreviousFieldCrop")]
        public FieldCropType[] GrownPreviousFieldCrop
        {
            get
            {
                return this.grownPreviousFieldCropField;
            }
            set
            {
                this.grownPreviousFieldCropField = value;
            }
        }

        public CropPlotType GrownCropPlot
        {
            get
            {
                return this.grownCropPlotField;
            }
            set
            {
                this.grownCropPlotField = value;
            }
        }

        [XmlElement("SpecifiedFieldCropMixtureConstituent")]
        public FieldCropMixtureConstituentType[] SpecifiedFieldCropMixtureConstituent
        {
            get
            {
                return this.specifiedFieldCropMixtureConstituentField;
            }
            set
            {
                this.specifiedFieldCropMixtureConstituentField = value;
            }
        }

        [XmlElement("HarvestedCropProduce")]
        public CropProduceType[] HarvestedCropProduce
        {
            get
            {
                return this.harvestedCropProduceField;
            }
            set
            {
                this.harvestedCropProduceField = value;
            }
        }

        [XmlElement("SpecifiedReferencedObservationResult")]
        public ReferencedObservationResultType[] SpecifiedReferencedObservationResult
        {
            get
            {
                return this.specifiedReferencedObservationResultField;
            }
            set
            {
                this.specifiedReferencedObservationResultField = value;
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

        [XmlElement("SpecifiedSingleAgronomicalObservation")]
        public SingleAgronomicalObservationType[] SpecifiedSingleAgronomicalObservation
        {
            get
            {
                return this.specifiedSingleAgronomicalObservationField;
            }
            set
            {
                this.specifiedSingleAgronomicalObservationField = value;
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

        [XmlElement("SpecifiedAgriculturalExtremeWeatherCondition")]
        public AgriculturalExtremeWeatherConditionType[] SpecifiedAgriculturalExtremeWeatherCondition
        {
            get
            {
                return this.specifiedAgriculturalExtremeWeatherConditionField;
            }
            set
            {
                this.specifiedAgriculturalExtremeWeatherConditionField = value;
            }
        }
    }
}