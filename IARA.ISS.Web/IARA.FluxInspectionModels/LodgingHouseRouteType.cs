namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseRoute", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseRouteType
    {

        private IDType idField;

        private TextType descriptionField;

        private BinaryObjectType[] mapBinaryObjectField;

        private TextType departurePointField;

        private TextType[] transportMeansField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("MapBinaryObject")]
        public BinaryObjectType[] MapBinaryObject
        {
            get => this.mapBinaryObjectField;
            set => this.mapBinaryObjectField = value;
        }

        public TextType DeparturePoint
        {
            get => this.departurePointField;
            set => this.departurePointField = value;
        }

        [XmlElement("TransportMeans")]
        public TextType[] TransportMeans
        {
            get => this.transportMeansField;
            set => this.transportMeansField = value;
        }
    }
}