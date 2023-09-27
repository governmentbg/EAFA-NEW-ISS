namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TravelProductReservationItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TravelProductReservationItemType
    {

        private QuantityType unitQuantityField;

        private IDType unitIDField;

        private TextType descriptionField;

        public QuantityType UnitQuantity
        {
            get => this.unitQuantityField;
            set => this.unitQuantityField = value;
        }

        public IDType UnitID
        {
            get => this.unitIDField;
            set => this.unitIDField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}