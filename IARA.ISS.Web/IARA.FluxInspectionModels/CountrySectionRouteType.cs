namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CountrySectionRoute", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CountrySectionRouteType
    {

        private TextType descriptionField;

        private MeasureType distanceMeasureField;

        private WaypointLocationType startWaypointLocationField;

        private WaypointLocationType endWaypointLocationField;

        private SectionRouteType[] subordinateSectionRouteField;

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public MeasureType DistanceMeasure
        {
            get => this.distanceMeasureField;
            set => this.distanceMeasureField = value;
        }

        public WaypointLocationType StartWaypointLocation
        {
            get => this.startWaypointLocationField;
            set => this.startWaypointLocationField = value;
        }

        public WaypointLocationType EndWaypointLocation
        {
            get => this.endWaypointLocationField;
            set => this.endWaypointLocationField = value;
        }

        [XmlElement("SubordinateSectionRoute")]
        public SectionRouteType[] SubordinateSectionRoute
        {
            get => this.subordinateSectionRouteField;
            set => this.subordinateSectionRouteField = value;
        }
    }
}