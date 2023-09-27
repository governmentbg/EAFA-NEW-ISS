namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalCrop", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalCropType
    {

        private CodeType botanicalSpeciesCodeField;

        private CodeType supplementaryBotanicalSpeciesCodeField;

        private CodeType sowingPeriodCodeField;

        private CodeType purposeCodeField;

        private CodeType plantingReasonCodeField;

        private TextType descriptionField;

        private CodeType developmentStageCodeField;

        private CropSpeciesVarietyType[] sownCropSpeciesVarietyField;

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

        public CodeType SupplementaryBotanicalSpeciesCode
        {
            get
            {
                return this.supplementaryBotanicalSpeciesCodeField;
            }
            set
            {
                this.supplementaryBotanicalSpeciesCodeField = value;
            }
        }

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

        public CodeType PurposeCode
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

        public CodeType PlantingReasonCode
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

        [XmlElement("SownCropSpeciesVariety")]
        public CropSpeciesVarietyType[] SownCropSpeciesVariety
        {
            get
            {
                return this.sownCropSpeciesVarietyField;
            }
            set
            {
                this.sownCropSpeciesVarietyField = value;
            }
        }
    }
}