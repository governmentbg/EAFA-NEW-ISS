namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseTravelProductComponent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseTravelProductComponentType
    {

        private CodeType categoryCodeField;

        private TextType categoryNameField;

        private IDType idField;

        private IndicatorType choiceAllowedIndicatorField;

        private TextType nameField;

        private TextType descriptionField;

        private ProductComponentItineraryType[] specifiedProductComponentItineraryField;

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public TextType CategoryName
        {
            get => this.categoryNameField;
            set => this.categoryNameField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IndicatorType ChoiceAllowedIndicator
        {
            get => this.choiceAllowedIndicatorField;
            set => this.choiceAllowedIndicatorField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("SpecifiedProductComponentItinerary")]
        public ProductComponentItineraryType[] SpecifiedProductComponentItinerary
        {
            get => this.specifiedProductComponentItineraryField;
            set => this.specifiedProductComponentItineraryField = value;
        }
    }
}