namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TradeProductFeature", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TradeProductFeatureType
    {

        private IDType idField;

        private TextType[] descriptionField;

        private TextType[] nameField;

        private CodeType typeCodeField;

        private TextType[] marketingPhraseField;

        private MeasureType[] marketingMeasureField;

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

        [XmlElement("Name")]
        public TextType[] Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
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

        [XmlElement("MarketingMeasure")]
        public MeasureType[] MarketingMeasure
        {
            get
            {
                return this.marketingMeasureField;
            }
            set
            {
                this.marketingMeasureField = value;
            }
        }
    }
}