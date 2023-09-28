namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("HazardRating", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class HazardRatingType
    {

        private TextType systemNameField;

        private TextType classificationTypeField;

        private TextType valueField;

        public TextType SystemName
        {
            get => this.systemNameField;
            set => this.systemNameField = value;
        }

        public TextType ClassificationType
        {
            get => this.classificationTypeField;
            set => this.classificationTypeField = value;
        }

        public TextType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}