namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GoodsProduction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GoodsProductionType
    {

        private IDType idField;

        private TextType[] manufacturingProcessDescriptionField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("ManufacturingProcessDescription")]
        public TextType[] ManufacturingProcessDescription
        {
            get => this.manufacturingProcessDescriptionField;
            set => this.manufacturingProcessDescriptionField = value;
        }
    }
}