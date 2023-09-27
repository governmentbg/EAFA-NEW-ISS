namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LivestockAnimalProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LivestockAnimalProductType
    {

        private TextType commonSpeciesNameField;

        private TextType scientificSpeciesNameField;

        private TextType descriptionField;

        private LivestockAnimalProductInstanceType individualLivestockAnimalProductInstanceField;

        public TextType CommonSpeciesName
        {
            get => this.commonSpeciesNameField;
            set => this.commonSpeciesNameField = value;
        }

        public TextType ScientificSpeciesName
        {
            get => this.scientificSpeciesNameField;
            set => this.scientificSpeciesNameField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public LivestockAnimalProductInstanceType IndividualLivestockAnimalProductInstance
        {
            get => this.individualLivestockAnimalProductInstanceField;
            set => this.individualLivestockAnimalProductInstanceField = value;
        }
    }
}