namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WaypointLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WaypointLocationType
    {

        private TextType nameField;

        private TextType typeField;

        private TextType descriptionField;

        private BasicGeographicalCoordinateType physicalBasicGeographicalCoordinateField;

        private PostCodeAddressType postalPostCodeAddressField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType Type
        {
            get => this.typeField;
            set => this.typeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public BasicGeographicalCoordinateType PhysicalBasicGeographicalCoordinate
        {
            get => this.physicalBasicGeographicalCoordinateField;
            set => this.physicalBasicGeographicalCoordinateField = value;
        }

        public PostCodeAddressType PostalPostCodeAddress
        {
            get => this.postalPostCodeAddressField;
            set => this.postalPostCodeAddressField = value;
        }
    }
}