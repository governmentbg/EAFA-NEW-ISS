namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("HotSpringsSource", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class HotSpringsSourceType
    {

        private TextType descriptionField;

        private IDType idField;

        private TextType marketingPhraseField;

        private TextType waterCharacteristicField;

        private TextType[] healthBenefitField;

        private TextType[] bathingProhibitionCautionField;

        private TextType drinkingProhibitionCautionField;

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

        public TextType MarketingPhrase
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

        public TextType WaterCharacteristic
        {
            get
            {
                return this.waterCharacteristicField;
            }
            set
            {
                this.waterCharacteristicField = value;
            }
        }

        [XmlElement("HealthBenefit")]
        public TextType[] HealthBenefit
        {
            get
            {
                return this.healthBenefitField;
            }
            set
            {
                this.healthBenefitField = value;
            }
        }

        [XmlElement("BathingProhibitionCaution")]
        public TextType[] BathingProhibitionCaution
        {
            get
            {
                return this.bathingProhibitionCautionField;
            }
            set
            {
                this.bathingProhibitionCautionField = value;
            }
        }

        public TextType DrinkingProhibitionCaution
        {
            get
            {
                return this.drinkingProhibitionCautionField;
            }
            set
            {
                this.drinkingProhibitionCautionField = value;
            }
        }
    }
}