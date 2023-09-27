namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FieldCropMixtureConstituent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FieldCropMixtureConstituentType
    {

        private PercentType cropProportionPercentField;

        private BotanicalCropType specifiedBotanicalCropField;

        public PercentType CropProportionPercent
        {
            get
            {
                return this.cropProportionPercentField;
            }
            set
            {
                this.cropProportionPercentField = value;
            }
        }

        public BotanicalCropType SpecifiedBotanicalCrop
        {
            get
            {
                return this.specifiedBotanicalCropField;
            }
            set
            {
                this.specifiedBotanicalCropField = value;
            }
        }
    }
}