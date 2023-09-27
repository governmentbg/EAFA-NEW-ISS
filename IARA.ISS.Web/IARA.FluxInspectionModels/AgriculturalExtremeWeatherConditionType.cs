namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalExtremeWeatherCondition", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalExtremeWeatherConditionType
    {

        private CodeType[] characteristicValueCodeField;

        private TextType[] characteristicValueField;

        [XmlElement("CharacteristicValueCode")]
        public CodeType[] CharacteristicValueCode
        {
            get
            {
                return this.characteristicValueCodeField;
            }
            set
            {
                this.characteristicValueCodeField = value;
            }
        }

        [XmlElement("CharacteristicValue")]
        public TextType[] CharacteristicValue
        {
            get
            {
                return this.characteristicValueField;
            }
            set
            {
                this.characteristicValueField = value;
            }
        }
    }
}