namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SolubilityGoodsCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SolubilityGoodsCharacteristicType
    {

        private TextType descriptionField;

        private TextType testMethodField;

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TextType TestMethod
        {
            get => this.testMethodField;
            set => this.testMethodField = value;
        }
    }
}