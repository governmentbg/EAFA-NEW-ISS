namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("EcologicalImpactGoodsCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class EcologicalImpactGoodsCharacteristicType
    {

        private TextType overviewDescriptionField;

        private TextType detailsDescriptionField;

        private TextType relatedImpactDescriptionField;

        private TextType testMethodField;

        public TextType OverviewDescription
        {
            get => this.overviewDescriptionField;
            set => this.overviewDescriptionField = value;
        }

        public TextType DetailsDescription
        {
            get => this.detailsDescriptionField;
            set => this.detailsDescriptionField = value;
        }

        public TextType RelatedImpactDescription
        {
            get => this.relatedImpactDescriptionField;
            set => this.relatedImpactDescriptionField = value;
        }

        public TextType TestMethod
        {
            get => this.testMethodField;
            set => this.testMethodField = value;
        }
    }
}