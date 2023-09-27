namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("BotanicalCrop", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class BotanicalCropType
    {

        private IDType botanicalIdentificationIDField;

        private CodeType botanicalSpeciesCodeField;

        private CodeType purposeCodeField;

        private CodeType botanicalGenusCodeField;

        private TextType botanicalNameField;

        private CropSpeciesVarietyType[] sownCropSpeciesVarietyField;

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