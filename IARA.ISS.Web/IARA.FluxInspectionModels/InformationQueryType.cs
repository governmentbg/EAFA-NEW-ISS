namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("InformationQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class InformationQueryType
    {

        private TextType contentField;

        public TextType Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }
    }
}