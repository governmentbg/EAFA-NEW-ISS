namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TravelProductFeature", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TravelProductFeatureType
    {

        private TextType[] keywordMarketingPhraseField;

        private TextType[] marketingPhraseField;

        private TextType descriptionField;

        [XmlElement("KeywordMarketingPhrase")]
        public TextType[] KeywordMarketingPhrase
        {
            get => this.keywordMarketingPhraseField;
            set => this.keywordMarketingPhraseField = value;
        }

        [XmlElement("MarketingPhrase")]
        public TextType[] MarketingPhrase
        {
            get => this.marketingPhraseField;
            set => this.marketingPhraseField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}