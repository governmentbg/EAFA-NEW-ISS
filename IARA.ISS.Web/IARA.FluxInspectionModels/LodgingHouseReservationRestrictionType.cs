namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseReservationRestriction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseReservationRestrictionType
    {

        private CodeType typeCodeField;

        private QuantityType minimumQuantityField;

        private QuantityType maximumQuantityField;

        private TextType descriptionField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public QuantityType MinimumQuantity
        {
            get => this.minimumQuantityField;
            set => this.minimumQuantityField = value;
        }

        public QuantityType MaximumQuantity
        {
            get => this.maximumQuantityField;
            set => this.maximumQuantityField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}