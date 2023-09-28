namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWRoute", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWRouteType
    {

        private TextType descriptionField;

        private BinaryObjectType mapBinaryObjectField;

        private MeasureType distanceMeasureField;

        private IDType coordinateSystemIDField;

        private WaypointLocationType startWaypointLocationField;

        private WaypointLocationType endWaypointLocationField;

        private CountrySectionRouteType[] nationalSubordinateCountrySectionRouteField;

        private SectionRouteType[] unstructuredSubordinateSectionRouteField;

        private TMWOrganizationType entryCustomsTMWOrganizationField;

        private TMWOrganizationType exitCustomsTMWOrganizationField;

        private TMWOrganizationType exportCustomsTMWOrganizationField;

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public BinaryObjectType MapBinaryObject
        {
            get => this.mapBinaryObjectField;
            set => this.mapBinaryObjectField = value;
        }

        public MeasureType DistanceMeasure
        {
            get => this.distanceMeasureField;
            set => this.distanceMeasureField = value;
        }

        public IDType CoordinateSystemID
        {
            get => this.coordinateSystemIDField;
            set => this.coordinateSystemIDField = value;
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

        [XmlElement("NationalSubordinateCountrySectionRoute")]
        public CountrySectionRouteType[] NationalSubordinateCountrySectionRoute
        {
            get => this.nationalSubordinateCountrySectionRouteField;
            set => this.nationalSubordinateCountrySectionRouteField = value;
        }

        [XmlElement("UnstructuredSubordinateSectionRoute")]
        public SectionRouteType[] UnstructuredSubordinateSectionRoute
        {
            get => this.unstructuredSubordinateSectionRouteField;
            set => this.unstructuredSubordinateSectionRouteField = value;
        }

        public TMWOrganizationType EntryCustomsTMWOrganization
        {
            get => this.entryCustomsTMWOrganizationField;
            set => this.entryCustomsTMWOrganizationField = value;
        }

        public TMWOrganizationType ExitCustomsTMWOrganization
        {
            get => this.exitCustomsTMWOrganizationField;
            set => this.exitCustomsTMWOrganizationField = value;
        }

        public TMWOrganizationType ExportCustomsTMWOrganization
        {
            get => this.exportCustomsTMWOrganizationField;
            set => this.exportCustomsTMWOrganizationField = value;
        }
    }
}