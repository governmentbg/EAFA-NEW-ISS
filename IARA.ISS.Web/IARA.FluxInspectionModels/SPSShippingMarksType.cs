namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSShippingMarks", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSShippingMarksType
    {

        private TextType markingField;

        public TextType Marking
        {
            get => this.markingField;
            set => this.markingField = value;
        }
    }
}