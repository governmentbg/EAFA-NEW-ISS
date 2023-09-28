namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GeographicalArea", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GeographicalAreaType
    {

        private IDType idField;

        private TextType nameField;

        private SpecifiedGeographicalCoordinateType actualSpecifiedGeographicalCoordinateField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public SpecifiedGeographicalCoordinateType ActualSpecifiedGeographicalCoordinate
        {
            get
            {
                return this.actualSpecifiedGeographicalCoordinateField;
            }
            set
            {
                this.actualSpecifiedGeographicalCoordinateField = value;
            }
        }
    }
}