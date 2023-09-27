namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseFeature", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseFeatureType
    {

        private IDType idField;

        private TextType localeNameField;

        private TextType[] marketingPhraseField;

        private TextType[] keywordMarketingPhraseField;

        private TextType descriptionField;

        private QuantityType occupantCapacityQuantityField;

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

        public TextType LocaleName
        {
            get
            {
                return this.localeNameField;
            }
            set
            {
                this.localeNameField = value;
            }
        }

        [XmlElement("MarketingPhrase")]
        public TextType[] MarketingPhrase
        {
            get
            {
                return this.marketingPhraseField;
            }
            set
            {
                this.marketingPhraseField = value;
            }
        }

        [XmlElement("KeywordMarketingPhrase")]
        public TextType[] KeywordMarketingPhrase
        {
            get
            {
                return this.keywordMarketingPhraseField;
            }
            set
            {
                this.keywordMarketingPhraseField = value;
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

        public QuantityType OccupantCapacityQuantity
        {
            get
            {
                return this.occupantCapacityQuantityField;
            }
            set
            {
                this.occupantCapacityQuantityField = value;
            }
        }
    }
}