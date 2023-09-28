namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTEventElement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTEventElementType
    {

        private IDType objectClassIDField;

        private QuantityType unitQuantityField;

        public IDType ObjectClassID
        {
            get => this.objectClassIDField;
            set => this.objectClassIDField = value;
        }

        public QuantityType UnitQuantity
        {
            get => this.unitQuantityField;
            set => this.unitQuantityField = value;
        }
    }
}