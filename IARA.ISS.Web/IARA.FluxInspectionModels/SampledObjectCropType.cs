namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampledObjectCrop", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampledObjectCropType
    {

        private IDType botanicalIdentificationIDField;

        private CodeType botanicalGenusCodeField;

        private CodeType botanicalSpeciesCodeField;

        private CodeType botanicalSpeciesVarietyCodeField;

        private CodeType classificationCodeField;

        private TextType[] descriptionField;

        private CodeType developmentStageCodeField;

        private CodeType cultivationTypeCodeField;

        private IndicatorType propagationMaterialIndicatorField;

        private MeasureType sizeMeasureField;

        private TextType botanicalNameField;

        private CodeType cultivationCoverageCodeField;

        private CodeType cultivationMediumCodeField;

        private CodeType cultivationContainerCodeField;

        private DateTimeType harvestDateTimeField;

        public IDType BotanicalIdentificationID
        {
            get
            {
                return this.botanicalIdentificationIDField;
            }
            set
            {
                this.botanicalIdentificationIDField = value;
            }
        }

        public CodeType BotanicalGenusCode
        {
            get
            {
                return this.botanicalGenusCodeField;
            }
            set
            {
                this.botanicalGenusCodeField = value;
            }
        }

        public CodeType BotanicalSpeciesCode
        {
            get
            {
                return this.botanicalSpeciesCodeField;
            }
            set
            {
                this.botanicalSpeciesCodeField = value;
            }
        }

        public CodeType BotanicalSpeciesVarietyCode
        {
            get
            {
                return this.botanicalSpeciesVarietyCodeField;
            }
            set
            {
                this.botanicalSpeciesVarietyCodeField = value;
            }
        }

        public CodeType ClassificationCode
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

        [XmlElement("Description")]
        public TextType[] Description
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

        public CodeType DevelopmentStageCode
        {
            get
            {
                return this.developmentStageCodeField;
            }
            set
            {
                this.developmentStageCodeField = value;
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

        public MeasureType SizeMeasure
        {
            get
            {
                return this.sizeMeasureField;
            }
            set
            {
                this.sizeMeasureField = value;
            }
        }

        public TextType BotanicalName
        {
            get
            {
                return this.botanicalNameField;
            }
            set
            {
                this.botanicalNameField = value;
            }
        }

        public CodeType CultivationCoverageCode
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
    }
}