namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SectionRoute", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SectionRouteType
    {

        private TextType descriptionField;

        private MeasureType distanceMeasureField;

        private WaypointLocationType startWaypointLocationField;

        private WaypointLocationType endWaypointLocationField;

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
    }
}